using UnityEngine;

public class AbilitiesContentPanel : MonoBehaviour
{
    public AbilitiesTopNavbarElement navbarElement;

    public void CloseContent()
    {
        navbarElement.selectedSprite.SetActive(false);
        navbarElement.isOpened = false;
        gameObject.SetActive(false);
    }

    public void OpenContent()
    {
        navbarElement.selectedSprite.SetActive(true);
        navbarElement.isOpened = true;
        gameObject.SetActive(true);
    }
}