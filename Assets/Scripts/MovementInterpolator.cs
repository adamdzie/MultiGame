using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class MovementInterpolator : NetworkBehaviour
{
    public Queue<PositionInTime> Buffer;

    public float elapsedTime;

    public Vector3 currentPosition;

    private bool fillStart;

    private PositionInTime past;
    private PositionInTime current;
    // static values

    public int fillThreshold = 4;

    public override void OnNetworkSpawn()
    {
        
        elapsedTime = 0f;
        fillStart = false;
        Buffer = new Queue<PositionInTime>();
    }
    private void Start()
    {
        currentPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClient)
        {

        CheckStartFill();

        Refresh();

            
        UpdateRotation();

        transform.position = currentPosition;

        if (gameObject.name == "Bullet") 
        Debug.Log($"W INTERPOLATORZE: {transform.position}");

        }

    }
    public void HandleNewTick(float time, Vector2 position)
    {
        Buffer.Enqueue(new PositionInTime(time, position));

    }
    private void Refresh()
    {
        if (!fillStart) return;

        elapsedTime += Time.deltaTime;

        //When elapsed time after past tick is bigger than current time tick 
        //If buffer queue is empty then extrapolate
        if(elapsedTime + past.Time >= current.Time)
        {
        
            if(Buffer.Count > 0)
            {
                elapsedTime = elapsedTime + past.Time - current.Time;
                past = current;
                current = Buffer.Dequeue();
            }
        
        }

        float _posX = (current.Position.x - past.Position.x) * (elapsedTime) / (current.Time - past.Time) + past.Position.x;
        float _posZ = (current.Position.y - past.Position.y) * (elapsedTime) / (current.Time - past.Time) + past.Position.y;

        currentPosition = new Vector3(_posX, transform.position.y, _posZ);

    }

    private void CheckStartFill()
    {
        if (!Application.isFocused) fillStart = false;
        if (fillStart) return;

        if(Buffer.Count >= fillThreshold)
        {
            past = Buffer.Dequeue();
            current = Buffer.Dequeue();
            past.Time = current.Time - 1 / 30;
            fillStart = true;
        } 

        

    }

    private void UpdateRotation()
    {

            Vector3 temp = transform.position;
            //temp.y = 0f;
            //Vector3 _relativepos = gameObject.GetComponent<PlayerController>().mousePoint.Value - temp;
            //currentPosition.y = 0f;
            Vector3 _relativepos = currentPosition - temp;


        Quaternion LookAtRotation = Quaternion.LookRotation(_relativepos);
        


            if (_relativepos != Vector3.zero)
            {
                transform.rotation = LookAtRotation;
            }
   
    }
    public struct PositionInTime : INetworkSerializable
    {
        public PositionInTime(float time, Vector2 position)
        {
            Time = time;
            Position = position;
        }

        public float Time;
        public Vector2 Position;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Time);
            serializer.SerializeValue(ref Position);
        }

        public override string ToString() => $"Time: {Time}, Position: {Position}";


    }

}
