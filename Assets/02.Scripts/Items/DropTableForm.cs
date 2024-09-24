using System.Collections.Generic;


namespace FoxHill.Items
{
    /// <summary>
    /// Excel Importer 사용 위해 자료형 변경한 클래스.
    /// 실제 게임 내에서는 DropTableForm으로 변환하여 사용.
    /// </summary>
    [System.Serializable]
    public class DropTableFormRaw
    {
        public int DropGroupIndexNumber;
        public int DropItem1;
        public int DropItem2;
        public int DropItem3;
        public int DropItem4;
        public int DropItem5;
        public float DropWeight1;
        public float DropWeight2;
        public float DropWeight3;
        public float DropWeight4;
        public float DropWeight5;
        public string MultipleDropWeightList;
    }

    public class DropTableForm
    {
        public int DropGroupIndexNumber;
        public int DropItem1;
        public int DropItem2;
        public int DropItem3;
        public int DropItem4;
        public int DropItem5;
        public float DropWeight1;
        public float DropWeight2;
        public float DropWeight3;
        public float DropWeight4;
        public float DropWeight5;
        public List<float> MultipleDropWeightList;
    }
}