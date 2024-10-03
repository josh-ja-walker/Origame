using UnityEngine;

public enum ColorPreset {
    Blue,
    Brown, 
    Cyan,
    Green,
    Orange,
    Pink,
    Purple,
    Yellow,
    Red,
    None
}

public static class ColorMethods {

    public static Color Paper(this ColorPreset preset) {
        return preset switch {
            ColorPreset.Blue => new Color32(0x66, 0xBB, 0xE5, 0xFF), //#66BBE5
            ColorPreset.Brown => new Color32(0xE5, 0x82, 0x66, 0xFF), //#E58266
            ColorPreset.Cyan => new Color32(0x66, 0xE5, 0xDC, 0xFF), //#66E5DC
            ColorPreset.Green => new Color32(0x72, 0xF9, 0x98, 0xFF), //#72F998
            ColorPreset.Orange => new Color32(0xFF, 0x8E, 0x47, 0xFF), //#FF8E47
            ColorPreset.Pink => new Color32(0xE5, 0x66, 0xA2, 0xFF), //#E566A2
            ColorPreset.Purple => new Color32(0xA8, 0x66, 0xE5, 0xFF), //#A866E5
            ColorPreset.Yellow => new Color32(0xE5, 0xDE, 0x66, 0xFF), //#E5DE66
            ColorPreset.Red => new Color32(0xE5, 0x66, 0x6F, 0xFF), //#E5666F
            _ => Color.magenta,
        };
    }

    public static Color Platform(this ColorPreset preset) {
        return preset switch {
            ColorPreset.Blue => new Color32(0x35, 0x9C, 0xCF, 0xFF), //#359CCF
            ColorPreset.Brown => new Color32(0xCF, 0x55, 0x35, 0xFF), //#CF5535
            ColorPreset.Cyan => new Color32(0x35, 0xCF, 0xC5, 0xFF), //#35CFC5
            ColorPreset.Green => new Color32(0x39, 0xE2, 0x6C, 0xFF), //#39E26C
            ColorPreset.Orange => new Color32(0xF1, 0x72, 0x2C, 0xFF), //#F1722C
            ColorPreset.Pink => new Color32(0xCF, 0x35, 0x7D, 0xFF), //#CF357D
            ColorPreset.Purple => new Color32(0x84, 0x35, 0xCF, 0xFF), //#8435CF
            ColorPreset.Yellow => new Color32(0xCF, 0xC7, 0x35, 0xFF), //#CFC735
            ColorPreset.Red => new Color32(0xCF, 0x35, 0x3F, 0xFF), //#CF353F
            _ => Color.magenta,
        };
    }

    
}