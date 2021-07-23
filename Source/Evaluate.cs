using System;
using System.Reflection;
using System.Xml;

using Verse;

namespace PatchOperation
{
    public class Evaluate : Verse.PatchOperation
    {
        string member = null;

        string value = "True";

        bool log = false;

        Verse.PatchOperation match = null;

        Verse.PatchOperation nomatch = null;

        [Unsaved]
        FieldInfo fieldInfo;

        [Unsaved]
        PropertyInfo propertyInfo;

        [Unsaved]
        Type memberType;

        [Unsaved]
        object memberValue;

        [Unsaved]
        bool initialized;

        bool Initialize()
        {
            if (initialized)
            {
                return memberType != null;
            }

            initialized = true;

            if (member.NullOrEmpty())
            {
                Error("member is null or empty");
                return false;
            }

            string[] names = member.Split(':');
            if (names.Length < 2)
            {
                Error("member has a bad format (type:member)");
                return false;
            }

            string typeName = names[0];
            string memberName = names[1];
            var type = GenTypes.GetTypeInAnyAssembly(typeName);
            if (type == null)
            {
                Error("member has no matching type in any assembly");
                return false;
            }

            var bindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

            fieldInfo = type.GetField(memberName, bindings);
            propertyInfo = type.GetProperty(memberName, bindings);

            if (fieldInfo != null)
            {
                memberType = fieldInfo.FieldType;
                memberValue = fieldInfo.GetValue(null);
            }
            else if (propertyInfo != null)
            {
                memberType = propertyInfo.PropertyType;
                memberValue = propertyInfo.GetValue(null);
            }
            else
            {
                Error("member is unavailable, only static field or property is allowed");
                return false;
            }
            
            if (log)
            {
                Log.Warning($"{member} = \"{memberValue}\" ({memberType})");
            }

            return true;
        }

        void Error(string message)
        {
            Log.Error($"{this} failed with {message}");
        }

        protected override bool ApplyWorker(XmlDocument xml)
        {
            if (!Initialize())
            {
                return false;
            }

            object parsedValue = ParseHelper.FromString(value, memberType);
            if (object.Equals(memberValue, parsedValue))
            {
                if (match != null)
				{
					return match.Apply(xml);
				}
            } else if (nomatch != null)
			{
				return nomatch.Apply(xml);
			}

            return true;
        }

        public override string ToString()
		{
            if (memberType == null)
            {
                return $"{base.ToString()}(\"{member}\", unknown == \"{value}\")";
            }
            else
            {
                return $"{base.ToString()}(\"{member}\", \"{memberValue}\" (${memberType}) == \"{value}\")";
            }
		}
    }
}