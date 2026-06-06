using System;

namespace _game {
    [Flags]
    public enum ItemType {
        None = 0,
        Acorn = 1 << 0,
        Apple_red = 1 << 1,
        Apple_green = 1 << 2,
        BlueBerry = 1 << 3,
        Carrot = 1 << 4,
        Cheese = 1 << 5,
        Cherry = 1 << 6,
        Cookie = 1 << 7,
        Corn = 1 << 8,
        Watermelon = 1 << 9,
        Fish = 1 << 10,
        Mushroom = 1 << 11,
        SodaBlue = 1 << 12,
        SodaOrange = 1 << 13,
        SodaRed = 1 << 14,
        All = ~0
    }
}