using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player
{
    public static List<Player> Players = new List<Player>();

    public float MoveSpeed { get; private set; }
    public float SprintSpeed { get; private set; }
    public float RotationSpeed { get; private set; }
    public float SpeedChangeRate { get; private set; }

    public float JumpHeight { get; private set; }
    public float Gravity { get; private set; }

    public bool Grounded { get; private set; }
    public float GroundedOffset { get; private set; }
    public float GroundedRadius { get; private set; }
    public LayerMask GroundLayers { get; private set; }

    public GameObject CinemachineCameraTarget { get; private set; }
    public float TopClamp { get; private set; }
    public float BottomClamp { get; private set; }

    public Image crosshairUI { get; private set; }
    public Image bulletsCounterUI { get; private set; }
    public Image takeToolUI { get; private set; }

    public Player(float MoveSpeed ,float SprintSpeed 
                  ,float RotationSpeed , float SpeedChangeRate
                  ,float JumpHeight , float Gravity
                  ,bool Grounded , float GroundedOffset
                  , float GroundedRadius, LayerMask GroundLayers
                  , GameObject CinemachineCameraTarget
                  , float TopClamp, float BottomClamp
                  , Image crosshair, Image bulletsCounter
                  , Image takeTool) 
    { 
        this.MoveSpeed = MoveSpeed;
        this.SprintSpeed = SprintSpeed;
        this.RotationSpeed = RotationSpeed;
        this.SpeedChangeRate = SprintSpeed;
        this.JumpHeight = JumpHeight;
        this.Gravity = Gravity; 
        this.Grounded = Grounded;
        this.GroundedOffset = GroundedOffset;
        this.GroundedRadius = GroundedRadius;
        this.GroundLayers = GroundLayers;
        this.CinemachineCameraTarget = CinemachineCameraTarget;
        this.TopClamp = TopClamp;
        this.BottomClamp = BottomClamp;
        this.crosshairUI = crosshair;
        this.bulletsCounterUI = bulletsCounter;
        this.takeToolUI = takeTool;

        Players.Add(this);
    }


}
