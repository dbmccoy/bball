using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    Character player;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Character>();
        player.onStartTurn.AddListener(StartTurn);
    }

    bool hasMoved;

    // Update is called once per frame
    void Update()
    {
        if (!hasMoved) {
            if (Input.GetKeyUp(KeyCode.D)) {
                if (player.MoveDir(Dir.E)) {
                    hasMoved = true;
                }
            }
            if (Input.GetKeyUp(KeyCode.A)) {
                if (player.MoveDir(Dir.W)) {
                    hasMoved = true;
                }
            }

            
            if(Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyCode.A)) {
                if (player.MoveDir(Dir.NW)) {
                    hasMoved = true;
                }
            }
            else if(Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.W)) {
                if (player.MoveDir(Dir.NW)) {
                    hasMoved = true;
                }
            }
            else if (Input.GetKeyUp(KeyCode.W)) {
                if (player.MoveDir(Dir.NW)) {
                    hasMoved = true;
                }
            }

            if (Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyCode.D)) {
                if (player.MoveDir(Dir.NE)) {
                    hasMoved = true;
                }
            }
            else if (Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.W)) {
                if (player.MoveDir(Dir.NE)) {
                    hasMoved = true;
                }
            }
            else if (Input.GetKeyUp(KeyCode.E)) {
                if (player.MoveDir(Dir.NE)) {
                    hasMoved = true;
                }
            }

            if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.A)) {
                if (player.MoveDir(Dir.SW)) {
                    hasMoved = true;
                }
            }
            else if (Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.S)) {
                if (player.MoveDir(Dir.SW)) {
                    hasMoved = true;
                }
            }
            else if (Input.GetKeyUp(KeyCode.Z)) {
                if (player.MoveDir(Dir.SW)) {
                    hasMoved = true;
                }
            }

            if (Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.D)) {
                if (player.MoveDir(Dir.SE)) {
                    hasMoved = true;
                }
            }
            else if (Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.S)) {
                if (player.MoveDir(Dir.SE)) {
                    hasMoved = true;
                }
            }
            else if (Input.GetKeyUp(KeyCode.X)) {
                if (player.MoveDir(Dir.SE)) {
                    hasMoved = true;
                }
            }
        }
    }

    public void StartTurn() {
        hasMoved = false;
    }

}
