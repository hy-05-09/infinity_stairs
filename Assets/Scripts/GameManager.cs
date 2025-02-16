using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool Game_Start = false; //게임 시작 체크

    public float Current_Time = 0.0f; //현재 남은 시간

    public float Destination_Time = 10.0f; //전체 시간

    public float Add_Time_Flow = 0.001f; //감소 시간

    public Slider Slider; //시간 타이머

    public Text Text; //점수 텍스트

    public int Score = 0; //점수

    public GameObject Character; //캐릭터
    public Transform Platform_Parents; //정리를 위한 발판들의 부모 오브젝트
    public GameObject Platform; //발판(계단)
    private List<GameObject> Platform_List = new  List<GameObject>(); //발판 리스트
    private List<int> Platform_Check_List = new List<int>(); //발판의 위치 리스트
    
    // Start is called before the first frame update
    void Start()
    {
        Data_Load();
        Init();
    }

    // Update is called once per frame
    void Update() //매 프레임마다 실행
    {
        if (Game_Start){ //키보드 입력 체크
            if (Input.GetKeyDown(KeyCode.RightArrow)){ //오른쪽 화살표 버튼을 눌렀을 때
                Check_Platform(Character_Pos_Idx, 1);
            }else if(Input.GetKeyDown(KeyCode.LeftArrow)){
                Check_Platform(Character_Pos_Idx, 0);
            }

            Destination_Time = Destination_Time - Add_Time_Flow;
            Current_Time = Current_Time - Time.deltaTime;


            Slider.value = Current_Time / Destination_Time;

            if(Current_Time < 0f){
                Result();
            }

        }else{
            if(Input.GetKeyDown(KeyCode.Space)){
                Init();
            }
        }
    }

    public void Data_Load() //데이터 로드. 발판 오브젝트 생성
    {

        for(int i=0; i<20; i++){
            GameObject t_Obj = Instantiate(Platform, Vector3.zero, Quaternion.identity);
            //복사하는 함수(복사할 오브젝트, 복제된 오브젝트 위치, 복제된 프로젝트 방향)
            //Vector3(a,b,c) 이때 a,b,c 값이 같으면 Vector3.0으로 표기 가능
            t_Obj.transform.parent = Platform_Parents;
            //Platform_Parents의 위치가 변하면 t_obj의 위치도 따라서 바뀐다
            Platform_List.Add(t_Obj); 
            Platform_Check_List.Add(0); //왼쪽이면 0, 오른쪽이면 1
        }
        Platform.SetActive(false); 
        //object 활성화 여부 결정 함수
        //복사가 완료되었기 때문에 더 이상 Platform 함수가 필요없으니 비활성화
    }

    private int Pos_Idx = 0; //발판의 마지막 위치를 저장하는 변수
    private int Character_Pos_Idx = 0; //캐릭터의 위치
    
    public void Init(){ //캐릭터 위치 초기화, 발판 위치 초기화
        Character.transform.position = Vector3.zero;
        //캐릭터 위치 초기화
        for(Pos_Idx = 0; Pos_Idx < 20;){
            Next_Platform(Pos_Idx);
        }

        Destination_Time = 10.0f;
        Current_Time = Destination_Time;

        Character_Pos_Idx = 0;
        Score = 0;
        Text.text = Score.ToString();

        Game_Start = true;
    }

    public void Next_Platform(int idx){
        int pos = Random.Range(0,2);
        //0보다 크거나 같고 2보다 작은 랜덤수 생성

        if (idx==0){ //처음 실행됐을 때
            Platform_List[idx].transform.position = new Vector3(0, -0.5f, 0);
        }else{
            if(Pos_Idx < 20){
                if(pos==0){ //왼쪽 발판일 경우
                    Platform_Check_List[idx-1] = pos;
                    Platform_List[idx].transform.position = Platform_List[idx-1].transform.position + new Vector3(-1f, 0.5f, 0);
                    //상대적인 위치 설정
                }else{ //오른쪽 발판일 경우
                    Platform_Check_List[idx-1] = pos;
                    Platform_List[idx].transform.position = Platform_List[idx-1].transform.position + new Vector3(1f, 0.5f, 0);
                }
            }else{
                if(pos==0){ //왼쪽 발판일 경우
                    if(idx % 20 == 0){ //idx가 20일 때
                        Platform_Check_List[20-1] = pos;
                        Platform_List[idx % 20].transform.position = Platform_List[20-1].transform.position + new Vector3(-1f, 0.5f, 0);
                        //상대적인 위치 설정
                    }else{ //idx가 20보다 클 때
                        Platform_Check_List[idx % 20 - 1] = pos;
                        Platform_List[idx % 20].transform.position = Platform_List[idx % 20-1].transform.position + new Vector3(-1f, 0.5f, 0);
                        //상대적인 위치 설정
                    }
                    
                }else{ //오른쪽 발판일 경우
                    if(idx % 20 == 0){ //idx가 20일 때
                        Platform_Check_List[20-1] = pos;
                        Platform_List[idx % 20].transform.position = Platform_List[20-1].transform.position + new Vector3(1f, 0.5f, 0);
                        //상대적인 위치 설정
                    }else{ //idx가 20보다 클 때
                        Platform_Check_List[idx % 20 - 1] = pos;
                        Platform_List[idx % 20].transform.position = Platform_List[idx % 20-1].transform.position + new Vector3(1f, 0.5f, 0);
                        //상대적인 위치 설정
                    }
                }
            }
        }
        Score++;
        Text.text = Score.ToString();
         Pos_Idx++;
    }
    void Check_Platform(int idx, int direction){
        Debug.Log("Idx : "+idx % 20 +" /Platform : "+Platform_Check_List[idx % 20] + " /Direction : "+direction);
        if(Platform_Check_List[idx % 20]==direction){ //캐릭터가 있는 방향에 발판이 있음
            Character_Pos_Idx++;
            Character.transform.position = Platform_List[Character_Pos_Idx % 20].transform.position + new Vector3(0f, 0.5f, 0);
            Next_Platform(Pos_Idx);
        }else{
            Result();
        }
    }

    public void Result(){
        Debug.LogFormat("Game Over");
        Game_Start = false;
    }
}
