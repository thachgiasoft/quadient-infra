namespace Infrastructure.Core.ComponentModel
{

    public enum SiralamaYonu { ASC = 1, DESC = 2 }
    public enum CharacterTypes
    {
        OnlyDigits,
        UpperLetters,
        LowerLetters,
        UpperLowerLetters,
        DigitLowerLetters,
        DigitUpperLetters,
        DigitUpperLowerLetters
    }
    public struct DataTypes
    {
        public const string Long = "System.Int64";
        public const string Int = "System.Int32";
        public const string Short = "System.Int16";
        public const string Decimal = "System.Decimal";
        public const string Double = "System.Double";
        public const string Byte = "System.Byte";
        public const string Boolean = "System.Boolean";
        public const string DateTime = "System.DateTime";
        public const string String = "System.String";
    }
}
