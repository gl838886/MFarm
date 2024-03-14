using Mfarm.AStar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mfarm.Save;
using Mono.Cecil;

public class NPCMovement : MonoBehaviour, ISaveable
{
    public ScheduleDataList_SO scheduleDataList;
    public SortedSet<ScheduleDetails> scheduleDetailsSet;
    public ScheduleDetails currentScheduleDetails;

    public string currentScene;
    private string targetScene;
    private Vector3Int currentGridPosition;
    private Vector3Int targetGridPosition;
    private Vector3Int nextGridPosition;
    private Vector3 nextWorldPosition;

    public string startScene { set => currentScene = value; }

    [Header("�ƶ�����")]
    public float normalSpeed = 2f;
    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    private Vector2 dir;
    public bool isMoving;

    private bool isFirstLoaded ; //����npc��·�Ͻ�����Ϸ
    private Season currentSeason;

    //npc���ϵ����
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;
    private Rigidbody2D rb;
    private Animator anim;
    public AnimatorOverrideController animOverride;
    public bool isInteractable;

    private Stack<MovementStep> movementStepsStack;

    //����
    private Grid grid;

    //ʱ��
    private TimeSpan gameTime => TimeManager.Instance.gameTime;

    public string GUID => GetComponent<DataGUID>().GUID;

    private bool isInitialized; //�Ƿ��ʼ��
    private bool npcMove;
    private bool sceneLoaded;

    
    //������ʱ��
    private float animationBreakTime;
    private bool canPlayStopAnimationClip;
    public AnimationClip blankAnimationClip;
    public AnimationClip stopAnimationClip;

    //npc�ƶ���Э��
    private Coroutine npcCoroutine;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        movementStepsStack= new Stack<MovementStep>();
        scheduleDetailsSet = new SortedSet<ScheduleDetails>();
        foreach(var scheduleDetail in scheduleDataList.scheduleDetailsList)
        {
            scheduleDetailsSet.Add(scheduleDetail);
        }

        //��animatorOverride���¸�ֵ��������ĵ�����
        animOverride = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = animOverride; 
    }

    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
    }

    private void OnEnable()
    {
        EventHandler.AfterLoadSceneEvent += OnAfterLoadSceneEvent;
        EventHandler.BeforeUnLoadSceneEvent += OnBeforeUnLoadSceneEvent;
        EventHandler.UpdateTimeEvent += OnUpdateTimeEvent;
        EventHandler.EndCurrentGame += OnEndCurrentGame;
    }

    private void OnDisable()
    {
        EventHandler.AfterLoadSceneEvent -= OnAfterLoadSceneEvent;
        EventHandler.BeforeUnLoadSceneEvent -= OnBeforeUnLoadSceneEvent;
        EventHandler.UpdateTimeEvent -= OnUpdateTimeEvent;
        EventHandler.EndCurrentGame -= OnEndCurrentGame;
    }


    private void OnUpdateTimeEvent(int minute, int hour, int day, Season season)
    {
        int Time = hour*100 + minute;
        ScheduleDetails matchScheduleDetails = null;
        foreach(var scheduleDetails in scheduleDetailsSet)
        {

            if(Time == scheduleDetails.Time)
            {
                if (day != scheduleDetails.day &&scheduleDetails.day != 0)
                    continue;
                if (season != scheduleDetails.season)
                    continue;
                matchScheduleDetails = scheduleDetails;
            }
            //��Ϊ�ǰ�ʱ������������������һ����֦
            else if(scheduleDetails.Time > Time)
            {
                break;
            }
        }
        if(matchScheduleDetails != null)
        {
            BuildPath(matchScheduleDetails);
            targetScene = matchScheduleDetails.targetScene;
        }
        currentSeason = season;
    }

    private void OnBeforeUnLoadSceneEvent()
    {
        sceneLoaded = false;
    }

    private void OnAfterLoadSceneEvent()
    {
        grid = FindObjectOfType<Grid>();
        CheckSceneAvaliable(); //���ؽ����鿴npc�Ƿ�����ڸó��ֵĳ�����
        if(!isInitialized)
        {
            isInitialized= true;
            InitNPC();
        }
        sceneLoaded= true;
        //Debug.Log(transform.name + " currentScene: " + currentScene + " targetScene: " + targetScene);
        if (!isFirstLoaded)
        {
            //�ڳ����������õ�����ԭ�ص�npc��λ��
            //��������һ��schedule
            currentGridPosition = grid.WorldToCell(transform.position);
            ScheduleDetails newSchedule = new ScheduleDetails(0, 0, 0, 0, currentSeason, targetScene, (Vector2Int)targetGridPosition, stopAnimationClip, isInteractable);
            BuildPath(newSchedule);
            isFirstLoaded = true;
            //��Ϸ��һ�ο�ʼ�������ػ�ִ�У�����ʱ��Ϊtargetscene��targetPosition���ǵ�ǰ�ĳ�����λ�ã���InitNPC������
            //�����൱�ڲ���
            //�����NPC�ƶ��б��棬isFirstLoaded��Ϊfalse���������¼���NPC��סʱ��ִ����������
            //���NPCû������Ҳ���õ��ģ���Ϊtargetscene��targetPosition�����ǳ�ʼλ��
        }
    }

    private void OnEndCurrentGame()
    {
        npcMove = false;
        //���������Ϸʱĳ��NPCǡ����Э���У���ֱ�ӽ���
        if(npcCoroutine != null)
        {
            StopCoroutine(npcCoroutine);
        }
    }

    private void Update()
    {
        if(sceneLoaded)
        {
            SwitchAnimation();
        }

        //��ʱ������
        animationBreakTime -= Time.deltaTime;
        canPlayStopAnimationClip = animationBreakTime < 0;
    }

    private void FixedUpdate()
    {
        if(sceneLoaded)
        {
            Movement();
        }
    }

    private void CheckSceneAvaliable()
    {
        if(currentScene == SceneManager.GetActiveScene().name)
        {
            SetActiveInScene();
        }
        else
        {
            SetInactiveInScene();
        }
    }

    private void SetActiveInScene()
    {
        spriteRenderer.enabled = true;
        coll.enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void SetInactiveInScene()
    {
        spriteRenderer.enabled = false;
        coll.enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    /// <summary>
    /// NPC����Ҫ�ƶ�ģ��
    /// </summary>
    private void Movement() //ÿ0.02s����һ��
    {
        if(!npcMove) //NPC�����ƶ�
        {
            if (movementStepsStack.Count > 0) //������в���Ҫ��
            {
                //����astar���Ǵ��յ㿪ʼ��ջ�ģ�����ջ�������������
                MovementStep step = movementStepsStack.Pop();
                currentScene = step.sceneName;
                CheckSceneAvaliable(); //�Ƿ�Ӧ�ó������������
                nextGridPosition = (Vector3Int)step.gridCoordinate;
                TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);
                MoveToGridPosition(nextGridPosition, stepTime);
            }
            else if(!isMoving && canPlayStopAnimationClip) //���һ��bool���ж��Ƿ�ֹ����ֹͣ��count<=0���ò��ֻ���fixedupdate�������
            {
                StartCoroutine(SetStopAnimation());
            }
        }
    }

    private void MoveToGridPosition(Vector3Int gridPos, TimeSpan stepTime)
    {
        //��õ�ǰ�����ƶ���Э��
        npcCoroutine =  StartCoroutine(MoveRoutine(gridPos, stepTime));
    }

    private IEnumerator MoveRoutine(Vector3Int gridPos, TimeSpan stepTime)
    {
        npcMove = true;
        nextWorldPosition = GetWorldPosition(gridPos);
        //����ʱ�����ƶ�
        if (stepTime > gameTime)
        {
            //�����ƶ���ʱ���
            float timeToMove = (float)(stepTime.TotalSeconds - gameTime.TotalSeconds);
            //ʵ���ƶ�����
            float distance= Vector3.Distance(transform.position, nextWorldPosition);
            
            //ʵ���ƶ��ٶ�
            float speed = Mathf.Max(minSpeed, (distance / timeToMove / Settings.timeThreshold));
            if (speed <=maxSpeed)
            {
                while(Vector3.Distance(transform.position, nextWorldPosition) > Settings.pixelSize)
                {
                    dir = (nextWorldPosition- transform.position).normalized;
                    Vector2 posOffset = new Vector2(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime);
                    rb.MovePosition(rb.position + posOffset);
                    yield return new WaitForFixedUpdate(); //��ֱͣ����һ��FixedUpdateִ��
                }
            }
        }
        //����Ѿ����˾�˲��
        rb.position = nextWorldPosition;
        currentGridPosition = gridPos;
        nextGridPosition = currentGridPosition;
        npcMove = false;
    }

    /// <summary>
    /// ��ʼ��NPC
    /// </summary>
    private void InitNPC()
    {
        targetScene = currentScene;

        //�����￪ʼʱ������������
        currentGridPosition = grid.WorldToCell(transform.position);
        transform.position = new Vector3(currentGridPosition.x + Settings.gridCellSize / 2f, currentGridPosition.y + Settings.gridCellSize / 2f, 0);
        targetGridPosition = currentGridPosition;
    }

    public void BuildPath(ScheduleDetails scheduleDetails) //ÿһ֡����һ��
    {
        movementStepsStack.Clear();
        targetScene = scheduleDetails.targetScene;
        currentScheduleDetails= scheduleDetails;
        targetGridPosition = (Vector3Int)scheduleDetails.targetGridPosition;
        stopAnimationClip = scheduleDetails.animationClipAtStop;
        this.isInteractable = scheduleDetails.isInteractable; //�Ƿ�ɽ���

        //��ͬһ�������ƶ�
        if (scheduleDetails.targetScene == currentScene) 
        {
            AStar.Instance.BuildPath(currentScene, (Vector2Int)currentGridPosition, scheduleDetails.targetGridPosition, movementStepsStack);
        }
        //�糡���ƶ�
        else if(scheduleDetails.targetScene != currentScene)
        {
            SceneRoute sceneRoute = NPCManager.Instance.GetSceneRoute(currentScene, scheduleDetails.targetScene);
            if(sceneRoute!= null)
            {
                //�ڱ༭���У�pathList��Ϊ��ѹջ�������Ƚ��������ǰ������յ�
                for(int i=0;i<sceneRoute.pathList.Count;i++) 
                {
                    Vector2Int fromPos, toPos;
                    ScenePath scenePath = sceneRoute.pathList[i];
                    //�����Դ�01_Field�ߵ�02_HomeΪ��
                    //1.����Ȼ����Ҫ��npc�ӵ�ǰλ���ߵ�01_Field���Ŵ�
                    //2.���Ŵ�02_Home���Ŵ��ߵ�Ŀ��λ��
                    //����1.��Ȼִ�е�һ���жϵ�if
                    //����2����Ȼִ�еڶ����жϵ�else
                    if (scenePath.fromGridCell.x > 999 || scenePath.fromGridCell.y > 999)
                    {
                        fromPos = (Vector2Int) currentGridPosition;
                    }
                    else
                    {
                        fromPos = scenePath.fromGridCell;
                    }
                    if(scenePath.goToGridCell.x > 999 || scenePath.goToGridCell.y > 999)
                    {
                        toPos= scheduleDetails.targetGridPosition;
                    }
                    else
                    {
                        toPos = scenePath.goToGridCell;
                    }
                    AStar.Instance.BuildPath(scenePath.sceneName, fromPos, toPos, movementStepsStack);
                }
            }
        }

        if(movementStepsStack.Count > 1)
        {
            //����ÿһ����Ӧ��ʱ���
            UpdateTimeOnPath();
        }
    }

    /// <summary>
    /// ��ջ�ڵ�ÿһ����ʱ����и�ֵ
    /// </summary>
    public void UpdateTimeOnPath()
    {
        MovementStep previousStep = null; //��һ��Ϊ�գ���ʱ����
        TimeSpan currentGameTime = gameTime;

        foreach(MovementStep step in movementStepsStack)
        {
            if(previousStep == null) //�����һ��Ϊ��
                previousStep= step;

            step.hour = currentGameTime.Hours;                                   
            step.minute = currentGameTime.Minutes;
            step.second = currentGameTime.Seconds;

            TimeSpan BetweenStepTime;
            if (IsInDiagonal(previousStep, step)) //���б����
            {
                //��Ϸ���������ʱ��=�������/�ٶ�/��Ϸ�ڵ�ʱ������
                BetweenStepTime = new TimeSpan (0, 0, (int)(Settings.gridCellDiagonalSize / normalSpeed / Settings.timeThreshold));
            }
            else
            {
                BetweenStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.timeThreshold));
                //     1/2f/0.002=250s
            }
            //ÿһ���ߵ�ʱ��
            currentGameTime = currentGameTime.Add(BetweenStepTime);
            previousStep= step;
        }
    }

    /// <summary>
    /// �ж������Ƿ�Ϊб��
    /// </summary>
    /// <param name="currentStep"></param>
    /// <param name="previosStep"></param>
    /// <returns></returns>
    private bool IsInDiagonal(MovementStep currentStep, MovementStep previosStep)
    {
        return (currentStep.gridCoordinate.x != previosStep.gridCoordinate.x) && (currentStep.gridCoordinate.y != previosStep.gridCoordinate.y);
    }

    /// <summary>
    /// �������귵����������
    /// </summary>
    /// <param name="gridPos"></param>
    /// <returns></returns>
    private Vector3 GetWorldPosition(Vector3Int gridPos)
    {
        Vector3 worldPos = grid.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize / 2f, worldPos.y + Settings.gridCellSize / 2f);
    }

    private void SwitchAnimation()
    {
        bool isMoving = transform.position != GetWorldPosition(targetGridPosition);
        anim.SetBool("isMoving", isMoving);
        if (isMoving)
        {
            anim.SetBool("ForceExit", true); //һ����ʼ��·��ǿ���Ƴ��ֽ׶εĶ���
            anim.SetFloat("dirX", dir.x);
            anim.SetFloat("dirY", dir.y);
        }
        else
        {
            anim.SetBool("ForceExit", false);
        }
    }

    private IEnumerator SetStopAnimation()
    {
        anim.SetFloat("dirX", 0);
        anim.SetFloat("dirY", -1);
        animationBreakTime = Settings.animationBreakTime; //��ԭ��ʱ��
        if (stopAnimationClip != null)
        {
            animOverride[blankAnimationClip] = stopAnimationClip;
            anim.SetBool("EventAnimation", true);
            yield return null;
            anim.SetBool("EventAnimation", false);
        }
        else
        {
            animOverride[stopAnimationClip] = blankAnimationClip;
            anim.SetBool("EventAnimation", false);
        }

        
    }

    public GameSaveData GenerateGameData()
    {
        GameSaveData saveData  =new GameSaveData();
        saveData.characterPosDict = new Dictionary<string, SerializableVector3>();
        saveData.characterPosDict.Add("currentPosition", new SerializableVector3(transform.position));
        saveData.characterPosDict.Add("targetPosition", new SerializableVector3(targetGridPosition));
        saveData.sceneName = currentScene;
        saveData.targetScene = targetScene;
        if(stopAnimationClip != null)
        {
            saveData.animationInstanceID = stopAnimationClip.GetInstanceID();
        }
        saveData.interactable = this.isInteractable;
        return saveData;
    }

    public void RestoreGameData(GameSaveData data)
    {
        currentScene = data.sceneName;
        targetScene = data.targetScene;
        isFirstLoaded = false;

        Vector3 currentPos = data.characterPosDict["currentPosition"].ToVector3();
        Vector3Int gridPos = (Vector3Int)data.characterPosDict["targetPosition"].ToVector2Int();

        transform.position = currentPos;
        targetGridPosition = gridPos;

        if(data.animationInstanceID != 0)
        {
            stopAnimationClip = (AnimationClip) Resources.InstanceIDToObject(data.animationInstanceID);
        }

        this.isInteractable = data.interactable;

        //���¼���NPC���Ѿ���ʼ��
        isInitialized= true; 
    }
}
