using UnityEngine;
using TMPro;

public class NumberPopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    public float moveYspeed = 20f;
    public float disappearTimer = 0f;
    private Color textColor;

    public static NumberPopup Create(Vector3 position, int amount, bool color)
    {
        Transform damagePopupTf = Instantiate(GameManager.Instance.damagePopup, position, Quaternion.identity);

        NumberPopup damagePopupScript = damagePopupTf.GetComponent<NumberPopup>();
        damagePopupScript.Setup(amount, color);

        return damagePopupScript;
    }
    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Setup(int damage, bool color)
    {
        Debug.Log("Damage");
        textMesh.SetText(damage.ToString());
        if (color) textMesh.color = Color.green;
        else textMesh.color = Color.red;

        textColor = textMesh.color;
    }

    public void Update()
    {
        transform.position += new Vector3(0, moveYspeed) * Time.deltaTime;

        disappearTimer -= Time.deltaTime;

        if(disappearTimer <= 0)
        {
            float disappearSpeed = 3f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if(textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
