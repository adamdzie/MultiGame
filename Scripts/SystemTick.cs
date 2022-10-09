using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class SystemTick
{
    public LinkedList<PositionInTime> TickPositionBuffer { get; }
    public float elapsedTimeAfterTick;
    public bool isFilled;
    private bool isFilledStart;
    private int startPositions;
    public Vector3 lastPosition;
    private float tickStamp;

    public SystemTick()
    {
        TickPositionBuffer = new LinkedList<PositionInTime>();
        elapsedTimeAfterTick = 0;
        isFilled = false;
        isFilledStart = false;
        startPositions = 3;
        lastPosition = new Vector3(0, 1, 0);
        tickStamp = 1 / 30;
    }

  /*  public Vector3 GetCurrentFramePosition()
    {
        if (!isFilled) return lastPosition;
        var _move = TickPositionBuffer.GetEnumerator();
        _move.MoveNext();

        PositionInTime _first = _move.Current;

        _move.MoveNext();

        PositionInTime _second = _move.Current;



        float _posX = (_second.Position.x - _first.Position.x) * (elapsedTimeAfterTick) / (_second.Time - _first.Time) + _first.Position.x;
        float _posY = (_second.Position.z - _first.Position.z) * (elapsedTimeAfterTick) / (_second.Time - _first.Time) + _first.Position.z;

        lastPosition = new Vector3(_posX, _first.Position.y, _posY);

        Debug.Log($"Current Position is: {lastPosition}");
        Debug.Log($"First is {_first.ToString()}");
        Debug.Log($"Second is {_second.ToString()}");
        Debug.Log($"Elapsed time after tick is : {elapsedTimeAfterTick}");
        Debug.Log($"W bufferze: {TickPositionBuffer.Count}");


        return lastPosition;
    }
    public void HandleNewTick(float time, Vector3 position)
    {
        if(TickPositionBuffer.Count == 1)
        {
            TickPositionBuffer.AddLast(new PositionInTime(time - tickStamp, TickPositionBuffer.First.Value.Position));
            TickPositionBuffer.RemoveFirst();
        }

        TickPositionBuffer.AddLast(new PositionInTime(time, position));

        if (!isFilledStart)
        {
            if (TickPositionBuffer.Count >= startPositions) isFilledStart = true;
        }
    }

    public void Refresh(float deltaTime)
    {
        if (TickPositionBuffer.Count == 0)
        {
            elapsedTimeAfterTick = 0f;
            isFilledStart = false;
        }
        if (!isFilledStart) return;

        if (TickPositionBuffer.Count > 1) isFilled = true;
        else isFilled = false;


        if (!isFilled)
        {
            Debug.Log($"Buffer is empty at {NetworkManager.Singleton.ServerTime.TimeAsFloat}");
            return;
        }



        var _move = TickPositionBuffer.GetEnumerator();
        _move.MoveNext();
        _move.MoveNext();

        float _second = _move.Current.Time;

        elapsedTimeAfterTick += deltaTime;

        if (TickPositionBuffer.First.Value.Time + elapsedTimeAfterTick >= _second)
        {

            elapsedTimeAfterTick = TickPositionBuffer.First.Value.Time + elapsedTimeAfterTick - _second;
            TickPositionBuffer.RemoveFirst();
            if (TickPositionBuffer.Count < 2) isFilled = false;
        }
    }
  */
    // With extrapolation
    // Not working properly

    public Vector3 GetCurrentFramePosition()
    {


        if (TickPositionBuffer.Count < 2) return lastPosition;

        if (TickPositionBuffer.First.Value.Position == TickPositionBuffer.Last.Value.Position) return TickPositionBuffer.First.Value.Position;

        elapsedTimeAfterTick += Time.deltaTime;

        float _posX = (TickPositionBuffer.Last.Value.Position.x - TickPositionBuffer.First.Value.Position.x) * (elapsedTimeAfterTick) / (TickPositionBuffer.Last.Value.Time - TickPositionBuffer.First.Value.Time) + TickPositionBuffer.First.Value.Position.x;
        float _posY = (TickPositionBuffer.Last.Value.Position.z - TickPositionBuffer.First.Value.Position.z) * (elapsedTimeAfterTick) / (TickPositionBuffer.Last.Value.Time - TickPositionBuffer.First.Value.Time) + TickPositionBuffer.First.Value.Position.z;

        lastPosition = new Vector3(_posX, TickPositionBuffer.First.Value.Position.y, _posY);

       Debug.Log($"Current Position is: {lastPosition}");
        Debug.Log($"First is {TickPositionBuffer.First.Value.ToString()}");
       Debug.Log($"Second is {TickPositionBuffer.Last.Value.ToString()}");
       Debug.Log($"Elapsed time after tick is : {elapsedTimeAfterTick}");
        Debug.Log($"W bufferze: {TickPositionBuffer.Count}");

        return lastPosition;
    }
    public void HandleNewTick(float time, Vector3 position)
    {
        float timeBetween = 0f; 

        if (TickPositionBuffer.Count > 1)
        {
            timeBetween = TickPositionBuffer.Last.Value.Time - TickPositionBuffer.First.Value.Time;
            TickPositionBuffer.RemoveFirst();

        }


        TickPositionBuffer.AddLast(new PositionInTime(time, position));

        if (TickPositionBuffer.Count < 2) return;

        

        if (TickPositionBuffer.First.Value.Position == TickPositionBuffer.Last.Value.Position)
        {
            elapsedTimeAfterTick = 0f;
            return;
        }
        elapsedTimeAfterTick = elapsedTimeAfterTick - timeBetween;
        //elapsedTimeAfterTick = (TickPositionBuffer.First.Value.Time + (lastPosition.x - TickPositionBuffer.First.Value.Position.x) / (TickPositionBuffer.Last.Value.Position.x - TickPositionBuffer.First.Value.Position.x) * (TickPositionBuffer.Last.Value.Time - TickPositionBuffer.First.Value.Time))-TickPositionBuffer.First.Value.Time;



    }





    public struct PositionInTime : INetworkSerializable{ 
      public PositionInTime(float time, Vector3 position)
    {
        Time = time;
        Position = position;
    }

        public float Time;
        public Vector3 Position;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Time);
            serializer.SerializeValue(ref Position);
        }

        public override string ToString() => $"Time: {Time}, Position: {Position}";


    }



}
