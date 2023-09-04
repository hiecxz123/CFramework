using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class StudentBase
{
    public int id;
    public string name;
    public int gender;
    public int classId;

}

public class StudentExp : StudentBase
{
    public string hobby;
    public float height;
    public float weight;
}

public class DeepCopyTest : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

        StudentBase sb = new StudentBase
        {
            id = 1,
            name = "a",
            classId = 1,
            gender = 0
        };

        StudentExp se = new StudentExp()
        {
            id = 2,
            name = "e",
            classId = 11,
            gender = 1,
            hobby = "play",
            height = 180,
            weight = 100
        };

        //ǳ��������
        StudentBase sb1 = new StudentBase
        {
            id = 10,
            name = "c",
            classId = 3,
            gender = 1
        };
        sb1 = sb;
        sb1.name = "b";
        //��� "b"
        //��Ȼ�޸ĵ���sb1��name���ԣ��������sb��name������"b"��ǳ������ֻ�ǿ��������ã�sb1�е�ֵ����sb��
        UnityEngine.Debug.Log(sb.name);
        sb1 = se;
        //��� "e"������ͬ��
        UnityEngine.Debug.Log(sb1.name);
        //���಻��ǿ��ת��������
        //StudentExp se1 = (StudentExp)sb;

        //�������
        //�������
        StudentBase sb_reflection1 = new StudentBase()
        {
            id = 22,
            name = "d",
            classId = 4,
            gender = 0
        };
        StudentBase sb_reflection2 = new StudentBase()
        {
            id = 33,
            name = "f",
            classId = 5,
            gender = 1
        };
        sb_reflection2 = CommonTools.TransReflection<StudentBase, StudentBase>(sb_reflection1);
        sb_reflection2.name = "change";
        //���d,change
        //ͨ��������������sb2��sb3�໥�����������
        UnityEngine.Debug.Log("sb_reflection1.name=" + sb_reflection1.name +
            "   sb_reflection2.name=" + sb_reflection2.name);
        //���л����
        StudentBase sb_Serialize1 = new StudentBase()
        {
            id = 22,
            name = "d",
            classId = 4,
            gender = 0
        };
        StudentBase sb_Serialize2 = new StudentBase()
        {
            id = 33,
            name = "f",
            classId = 5,
            gender = 1
        };
        sb_Serialize2 = CommonTools.TransSerialize<StudentBase, StudentBase>(sb_Serialize1);
        sb_Serialize2.name = "change";
        UnityEngine.Debug.Log("sb_Serialize1.name=" + sb_Serialize1.name +
            "   sb_Serialize2.name=" + sb_Serialize2.name);
        //���ʽ�����
        StudentBase sb_exp1 = new StudentBase()
        {
            id = 22,
            name = "d",
            classId = 4,
            gender = 0
        };
        StudentBase sb_exp2 = new StudentBase()
        {
            id = 33,
            name = "f",
            classId = 5,
            gender = 1
        };

        sb_exp2 = CommonTools.TransExp<StudentBase, StudentBase>.Trans(sb_exp1);
        sb_exp2.name = "change";
        UnityEngine.Debug.Log("sb_exp1.name=" + sb_exp1.name +
            "   sb_exp2.name=" + sb_exp2.name);

        //���ܲ��� 100w���� 
        List<StudentBase> studentBases = new List<StudentBase>();
        for (int i = 0; i < 1000000; i++)
        {
            StudentBase temp = new StudentBase()
            {
                id = i,
                name = "name_" + i,
                classId = i % 10,
                gender = i % 2
            };
            studentBases.Add(temp);
        }
        Stopwatch sw = new Stopwatch();

        //���� 1387ms
        sw.Start();
        for (int i = 0; i < studentBases.Count; i++)
        {
            StudentBase temp = CommonTools.TransReflection<StudentBase, StudentBase>(studentBases[i]);
        }
        sw.Stop();
        UnityEngine.Debug.Log(string.Format("Reflection total:{0} ms", sw.ElapsedMilliseconds));
        sw.Reset();

        //���л� 5020ms
        sw.Start();
        for (int i = 0; i < studentBases.Count; i++)
        {
            StudentBase temp = CommonTools.TransSerialize<StudentBase, StudentBase>(studentBases[i]);
        }
        sw.Stop();
        UnityEngine.Debug.Log(string.Format("Serialize total:{0} ms", sw.ElapsedMilliseconds));
        sw.Reset();


        //���ʽ�� 73ms
        sw.Start();
        for (int i = 0; i < studentBases.Count; i++)
        {
            StudentBase temp = CommonTools.TransExp<StudentBase, StudentBase>.Trans(studentBases[i]);
        }
        sw.Stop();
        UnityEngine.Debug.Log(string.Format("Exp total:{0} ms", sw.ElapsedMilliseconds));
        sw.Reset();

        //ֱ�Ӹ�ֵ 147ms
        sw.Start();
        for (int i = 0; i < studentBases.Count; i++)
        {
            StudentBase temp = new StudentBase()
            {
                id = studentBases[i].id,
                name = studentBases[i].name,
                gender = studentBases[i].gender,
                classId = studentBases[i].classId
            };
        }
        sw.Stop();
        UnityEngine.Debug.Log(string.Format("copy total:{0} ms", sw.ElapsedMilliseconds));
        sw.Reset();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
