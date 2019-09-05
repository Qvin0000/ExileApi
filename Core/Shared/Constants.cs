using SharpDX;

namespace Shared
{
    public static class Constants
    {
        public static readonly Size2F MapIconsSize = new Size2F(14, 14);
        public static readonly Size2F MyMapIcons = new Size2F(7, 8);

        public static readonly uint[] PlayerXpLevels =
        {
            0u, 0u, 525u, 1760u, 3781u, 7184u, 12186u, 19324u, 29377u, 43181u, 61693u, 85990u, 117506u, 157384u, 207736u, 269997u,
            346462u, 439268u, 551295u, 685171u, 843709u, 1030734u, 1249629u, 1504995u, 1800847u, 2142652u, 2535122u, 2984677u, 3496798u,
            4080655u, 4742836u, 5490247u, 6334393u, 7283446u, 8348398u, 9541110u, 10874351u, 12361842u, 14018289u, 15859432u, 17905634u,
            20171471u, 22679999u, 25456123u, 28517857u, 31897771u, 35621447u, 39721017u, 44225461u, 49176560u, 54607467u, 60565335u,
            67094245u, 74247659u, 82075627u, 90631041u, 99984974u, 110197515u, 121340161u, 133497202u, 146749362u, 161191120u,
            176922628u, 194049893u, 212684946u, 232956711u, 255001620u, 278952403u, 304972236u, 333233648u, 363906163u, 397194041u,
            433312945u, 472476370u, 514937180u, 560961898u, 610815862u, 664824416u, 723298169u, 786612664u, 855129128u, 929261318u,
            1009443795u, 1096169525u, 1189918242u, 1291270350u, 1400795257u, 1519130326u, 1646943474u, 1784977296u, 1934009687u,
            2094900291u, 2268549086u, 2455921256u, 2658074992u, 2876116901u, 3111280300u, 3364828162u, 3638186694u, 3932818530u,
            4250334444u
        };

        public static readonly uint[] ToraExperienceLevels = new uint[] {0, 770, 0xa82, 0x245e, 0x7de6, 0x18810, 0x4b5d8, 0xe42b8};
        public static readonly uint[] CatarinaExperienceLevels = new uint[] {0, 770, 0xa82, 0x245e, 0x7de6, 0x18810, 0x4b5d8, 0xe42b8};
        public static readonly uint[] HakuExperienceLevels = new uint[] {0, 430, 0x668, 0x186a, 0x5cda, 0x13d88, 0x430a0, 0xdee87};
        public static readonly uint[] VaganExperienceLevels = new uint[] {0, 0x41a, 0xd98, 0x2cec, 0x9448, 0x1b867, 0x50bd8, 0xe9243};
        public static readonly uint[] VoriciExperienceLevels = new uint[] {0, 0x41a, 0xd98, 0x2cec, 0x9448, 0x1b867, 0x50bd8, 0xe9243};
        public static readonly uint[] ElreonExperienceLevels = new uint[] {0, 430, 0x668, 0x186a, 0x5cda, 0x13d88, 0x430a0, 0xdee87};
        public static readonly uint[] ZanaExperienceLevels = new uint[] {0, 0x125c, 0x3372, 0x9006, 0x1935c, 0x4696a, 0xc5a62, 0x2296ba};
        public static readonly uint[] LeoExperienceLevels = new uint[] {0, 0xa8c, 0x1b76, 0x475e, 0xb9a0, 0x1b264, 0x3ebf8, 0x8ec06};

        public static int GetLevel(uint[] expLevels, uint currExp) {
            for (var i = 1; i < expLevels.Length; i++)
                if (currExp < expLevels[i])
                    return i;
            return 8;
        }

        public static uint GetFullCurrentLevel(uint[] expLevels, uint currExp) {
            uint fullLevel = 0;
            for (var i = 1; i < expLevels.Length; i++)
            {
                var level = expLevels[i];
                if (currExp < level)
                {
                    fullLevel += level;
                    return fullLevel;
                }
                else
                    fullLevel += level;
            }

            return 8;
        }
    }
}