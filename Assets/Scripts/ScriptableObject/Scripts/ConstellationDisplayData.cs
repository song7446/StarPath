using System;

// 💡 [Serializable]을 붙여두면 나중에 유니티 인스펙터 창에서 
// 이 데이터가 제대로 들어갔는지 디버깅용으로 훔쳐볼 수 있어서 아주 편합니다!
[Serializable]
public class ConstellationDisplayData
{
    public ConstellationRow Row;
    public ConstellationData Data;
}