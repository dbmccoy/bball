using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    private static Mouse _instance;
    public static Mouse Instance {
        get {
            if(_instance == null) {
                _instance = Camera.main.GetComponent<Mouse>();
            }
            return _instance;
        }
    }

    public enum Mode {
        move,
        pass,
        shoot
    }

    public Mode mode;

    public Character CurrentPlayer;

    public GameObject SelectHex;

    public LayerMask HexMask;
    public LayerMask PlayerMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetMode(string s) {
        if(s == "move") {
            mode = Mode.move;
        }
        if(s == "shoot") {
            mode = Mode.shoot;
        }
        if(s == "pass") {
            mode = Mode.pass;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 100f, PlayerMask)) {
            if (Input.GetMouseButtonDown(0)) {
                CurrentPlayer = hitInfo.transform.GetComponentInParent<Character>();
                CurrentPlayer.Select();
                Debug.Log("selected player");
            }
        }

        else if (Physics.Raycast(ray, out hitInfo,100f,HexMask)){

            SelectHex.transform.position = hitInfo.transform.position;

            var Hex = hitInfo.transform.GetComponentInParent<Hex>();

            Hex.MouseOver();
            if (CurrentPlayer) {
                CurrentPlayer.nav.SetGoal(Hex, false);
            }

            if (Input.GetMouseButtonUp(0)) {
                if (CurrentPlayer) {
                    //CurrentPlayer.MoveTo(Hex);
                    if(mode == Mode.move) {
                        CurrentPlayer.SetPath(CurrentPlayer.nav.Path);
                        CurrentPlayer.movePointsRemaining -= CurrentPlayer.nav.Path.Count;
                    }
                }
            }
        }
        else {
            SelectHex.transform.position = new Vector3(0, 100, 0);
        }
    }
}
