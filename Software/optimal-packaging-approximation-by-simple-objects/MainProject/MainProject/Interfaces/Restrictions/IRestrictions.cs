using hs071_cs;

namespace MainProject.Interfaces
{
    public interface IRestrictions
    {
        void AmountOfIntersectionElement(Data data, out int amountOfElementThoseMustNotIntersect, out int _nele_jacAmountOfElementThoseMustNotIntersect);

        int ValueRestriction(Data data, ref int countObjects, double[] _x_L, double[] _x_U);
    }
}
