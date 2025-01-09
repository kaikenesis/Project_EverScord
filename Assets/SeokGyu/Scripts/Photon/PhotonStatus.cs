using UnityEngine;

[SerializeField]
public class PhotonStatus
{
    public string playerName { get; private set; }
    public int status { get; private set; }
    public string message { get; private set; }

    public PhotonStatus(string name, int status, string message)
    {
        playerName = name;
        this.status = status;
        this.message = message;
    }
}
