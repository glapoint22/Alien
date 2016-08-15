using UnityEngine;

//public class Player: Character{
public class Player{
    public Movement movement;
    public PlayerAnimation animation;
    public Commands commands = new Commands();
    public Transform transform;
    public Camera camera;
    private Transform eyes;

    public Player(Transform transform)
    {
        this.transform = transform;
        animation = new PlayerAnimation(transform.GetComponent<Animator>());
        

        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        Transform[] children = transform.GetComponentsInChildren<Transform>();

        foreach(Transform child in children)
        {
            if(child.name == "Eyes")
            {
                eyes = child;
                break;
            }
        }


        movement = new Movement(transform, eyes);

    }

    

    public void SetCamera()
    {
        camera.enabled = true;
        camera.transform.SetParent(eyes);
        camera.transform.localPosition = Vector3.zero;
        camera.transform.localRotation = Quaternion.identity;
    }
}
