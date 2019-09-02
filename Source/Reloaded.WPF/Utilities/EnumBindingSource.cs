using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Reloaded.WPF.Utilities
{
    public class EnumBindingSourceExtension : MarkupExtension
    {
        /// <summary>
        /// Defines the type of the enumerable to generate bindable items from.
        /// </summary>
        public Type EnumType
        {
            get => _enumType;
            set
            {
                if (value == _enumType)
                    return;

                if (value != null)
                {
                    Type enumType = Nullable.GetUnderlyingType(value) ?? value;
                    if (!enumType.IsEnum)
                        throw new ArgumentException("Not an enumerable type.");
                }

                this._enumType = value;
            }
        }

        private Type _enumType;

        public EnumBindingSourceExtension() { }
        public EnumBindingSourceExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        /// <summary>
        /// Retrieves the list of bindable items.
        /// </summary>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (null == this._enumType)
                throw new InvalidOperationException($"The {nameof(EnumType)} must be specified.");

            Type actualEnumType = Nullable.GetUnderlyingType(this._enumType) ?? this._enumType;
            Array enumValues    = Enum.GetValues(actualEnumType);

            if (actualEnumType == this._enumType)
                return enumValues;

            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }
    }
}
