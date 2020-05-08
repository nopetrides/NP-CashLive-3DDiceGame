using UnityEngine;
using UnityEngine.UIElements;

public class DieBehaviour : MonoBehaviour
{
    // Serialize the fields so they can come from a designer who edits them in the prefab
    [SerializeField]
    private Rigidbody m_RigidBody = null;
    [SerializeField]
    private DieManager m_Manager = null;
    [SerializeField]
    private Transform[] m_SideLocators = null;

    private Vector3[] m_PositionalDataFromLastframes;
    private bool m_IsMoving = true;
    private int m_HasBeenStillForFrameCount = 0;

    public bool IsMoving()
    {
        return m_IsMoving;
    }

    void OnEnable()
    {
        m_PositionalDataFromLastframes = new Vector3[m_Manager.GetMaxNoMoveFrameCount()];
        for (int i = 0; i < m_PositionalDataFromLastframes.Length; i++)
        {
            m_PositionalDataFromLastframes[i] = Vector3.zero;
        }
    }

    /// <summary>
    /// When we find that the velocity of the rigidbody is not high enough to be considered moving
    /// we just set its movement to zero. The Object could still be hit, and bounce away but thats okay.
    /// </summary>
    void Update()
    {
        if (m_RigidBody.velocity.magnitude < m_Manager.GetMinimumMovement())
        {
            m_HasBeenStillForFrameCount++;
        }
        else
        {
            m_HasBeenStillForFrameCount = 0;
        }
        if (m_HasBeenStillForFrameCount > m_Manager.GetMaxNoMoveFrameCount())
        {
            m_IsMoving = false;
            m_RigidBody.velocity = Vector3.zero;
        }
        else
        {
            m_IsMoving = true;
        }
    }

    public void Roll()
    {
        m_IsMoving = true;
        m_HasBeenStillForFrameCount = 0;
        m_RigidBody.AddForce(m_Manager.GetStaticRollForce(), ForceMode.Impulse);
        Random.InitState(System.DateTime.Now.Millisecond);
        Vector3 rotation = new Vector3(
            GetRandRotSpeed(),
             GetRandRotSpeed(),
              GetRandRotSpeed());
        m_RigidBody.AddTorque(rotation, ForceMode.Impulse);
    }

    private float GetRandRotSpeed()
    {
        float returnValue = Random.Range(m_Manager.GetMinDieRotationSpeed(), m_Manager.GetMaxDieRotationSpeed());
        returnValue = ((int)returnValue % 2 == 0) ? returnValue : returnValue * -1; // random chance to set it as negative
        return returnValue;
    }

    public int GetScore()
    {
        int highestFacingDieSide = 0;  // 0 is error state
        Vector3 highestPos = Vector3.zero;
        for (int i = 0; i < m_SideLocators.Length; i++)
        {
            if (m_SideLocators[i].position.y > highestPos.y)
            {
                highestPos = m_SideLocators[i].position;
                highestFacingDieSide = i+1;
            }
        }

        return highestFacingDieSide;
    }

    public void SetRandomStartingRotation()
    {
        Quaternion rotation = new Quaternion(
           GetRandomQuaternion(),
            GetRandomQuaternion(),
             GetRandomQuaternion(),
             1);
        m_RigidBody.rotation = rotation.normalized;
    }

    private float GetRandomQuaternion()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        float q = Random.Range(-1.0f,1.0f);
        Debug.Log(q);
        return q;
    }
}
