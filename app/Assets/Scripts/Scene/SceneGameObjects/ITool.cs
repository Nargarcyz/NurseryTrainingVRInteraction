public interface ITool {
    Tools GetToolType();
}

public enum Tools
{
    Scissors01,
    Scissors02,
    Forceps01,
    Forceps02,
    Clamp01,

    Pinset_recto,
    Forceps_Hoskin,
    Forceps_McPherson,
    Forceps_Anatomical,
    Mosquito_Kocher_Small,
    Mosquito_Kocher_Large,
}