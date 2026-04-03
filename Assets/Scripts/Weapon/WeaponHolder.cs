using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [Header("位置信息")]
    [SerializeField] private Transform rightHand;
    [SerializeField] private Transform backMount;
    [SerializeField] private GameObject weapon;

    [Header("武器偏移")]
    [SerializeField] private Vector3 handPosOffset;
    [SerializeField] private Vector3 handRotOffset;

    [Header("收刀偏移")]
    [SerializeField] private Vector3 backPosOffset;
    [SerializeField] private Vector3 backRotOffset;

    [SerializeField] private bool isWeaponDrawn = true;

    void Start()
    {
        //DrawWeapon();
        //SheathWeapon();
    }

    /// <summary>
    /// 持有状态偏移
    /// </summary>
    public void DrawWeapon()
    {
        weapon.transform.SetParent(rightHand);
        weapon.transform.localPosition = handPosOffset;
        weapon.transform.localEulerAngles = handRotOffset;
        isWeaponDrawn = true;
    }

    /// <summary>
    /// 收起状态偏移
    /// </summary>
    public void SheathWeapon()
    {
        weapon.transform.SetParent(backMount);
        weapon.transform.localPosition = backPosOffset;
        weapon.transform.localEulerAngles = backRotOffset;
        isWeaponDrawn = false;
    }

    /// <summary>
    /// 用于Attack2的剑的反转
    /// </summary>
    public void RotateWeapon(Vector3 rotation)
    {
        this.transform.localRotation = Quaternion.Euler(rotation);
    }
    
}
