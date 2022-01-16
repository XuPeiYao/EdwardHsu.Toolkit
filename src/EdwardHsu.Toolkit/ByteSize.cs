namespace EdwardHsu.Toolkit
{
    public class ByteSize
    {
        public const int BYTE_BITS = 8;

        public const int Kilo = 1000;
        public const int Kibi = 1024;

        public const int KB = Kilo;
        public const int KiB = Kibi;

        public const int MB = KB * Kilo;
        public const int MiB = KiB * Kibi;

        public const long GB = MB * Kilo;
        public const long GiB = MiB * Kibi;

        public const long TB = GB * Kilo;
        public const long TiB = GiB * Kibi;

        public const long PB = TB * Kilo;
        public const long PiB = TiB * Kibi;
    }
}
