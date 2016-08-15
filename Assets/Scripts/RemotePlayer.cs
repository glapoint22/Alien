using UnityEngine;

public class RemotePlayer : MonoBehaviour {
    public PlayerData[] playerData = new PlayerData[10];
    private float backTime;
    private int interpolatedIndex = 0;
    private float extrapolateTime = 0;
    private int index = 0;

    

    private Player player;

    void Start()
    {
        PlayerManager playerManager = GetComponent<PlayerManager>();

        player = playerManager.player;
        backTime = playerManager.backTime;
    }
    

    public void SetPlayerData(PlayerData data)
    {

        playerData[index] = data;

        //Set the time
        playerData[index].time = Time.time;


        //Increment the index
        index++;
        index = index % playerData.Length;

        //Move the last entry in the array to the first
        //This needs to happen because the last entry never gets looked at
        if (index == 0 && playerData[playerData.Length - 1].time != 0)
        {
            playerData[index] = playerData[playerData.Length - 1];
            index++;

        }
    }

    void Update()
    {
        //Interpolate
        for (int i = playerData.Length - 2; i > -1; i--)
        {
            if (playerData[i].time != 0 && playerData[i].time <= Time.time - backTime && playerData[i + 1].time > Time.time - backTime)
            {
                //Calculate the fraction
                float a = playerData[i + 1].time - playerData[i].time;
                float b = (Time.time - backTime) - playerData[i].time;
                float t = b / a;
                

                //Position and rotation
                transform.position = Vector3.Lerp(playerData[i].position, playerData[i + 1].position, t);
                transform.rotation = Quaternion.Lerp(playerData[i].rotation, playerData[i + 1].rotation, t);

                //Animation
                player.animation.forward_Backward = Mathf.Lerp(playerData[i].animation.forward_Backward, playerData[i + 1].animation.forward_Backward, t);
                player.animation.left_Right = Mathf.Lerp(playerData[i].animation.left_Right, playerData[i + 1].animation.left_Right, t);
                player.animation.isGrounded = playerData[i].animation.isGrounded;
                player.animation.jump = Mathf.Lerp(playerData[i].animation.jump, playerData[i + 1].animation.jump, t);


                //Update varialbes
                interpolatedIndex = i;
                extrapolateTime = 0;
                return;
            }
        }


        //Extrapolate
        if (playerData[interpolatedIndex].time != 0 && playerData[interpolatedIndex].time < (Time.time - backTime) && extrapolateTime < .25)
        {
            //Increment the extrapolate time. We don't want to go over .25 sec
            extrapolateTime += Time.deltaTime;

            //Extrapolate the position
            transform.position += (playerData[interpolatedIndex + 1].position - playerData[interpolatedIndex].position) / (playerData[interpolatedIndex + 1].time - playerData[interpolatedIndex].time) * Time.deltaTime;


            //Extrapolate the rotation
            Quaternion rot = (playerData[interpolatedIndex + 1].rotation * Quaternion.Inverse(playerData[interpolatedIndex].rotation));
            float t = (playerData[interpolatedIndex + 1].time - playerData[interpolatedIndex].time);
            float ang = 0.0f;
            Vector3 axis = Vector3.zero;
            rot.ToAngleAxis(out ang, out axis);
            if (ang > 180) ang -= 360;
            ang = ang * t % 360;
            transform.rotation *= Quaternion.AngleAxis(ang, axis);
        }
    }
}
