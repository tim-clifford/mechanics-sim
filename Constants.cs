using System;
namespace Program {
    static class Constants {
        // Constants and conversions to the universal implicit units of metres and radians
        public static readonly double AU = 1.495978707e11;
        public static readonly double deg = Math.PI/180;
        public static readonly double G = 6.67e-11;
        
    }
    static class Globals {
        public static Graphics.SystemWindow active_syswindow;
        public static int collision_counter = 0;
    }
}