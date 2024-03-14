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

    [Header("移动属性")]
    public float normalSpeed = 2f;
    public float minSpeed = 1f;
    public float maxSpeed = 3f;
    private Vector2 dir;
    public bool isMoving;

    private bool isFirstLoaded ; //用于npc在路上结束游戏
    private Season currentSeason;

    //npc身上的组件
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D coll;
    private Rigidbody2D rb;
    private Animator anim;
    public AnimatorOverrideController animOverride;
    public bool isInteractable;

    private Stack<MovementStep> movementStepsStack;

    //网格
    private Grid grid;

    //时间
    private TimeSpan gameTime => TimeManager.Instance.gameTime;

    public string GUID => GetComponent<DataGUID>().GUID;

    private bool isInitialized; //是否初始化
    private bool npcMove;
    private bool sceneLoaded;

    
    //动画计时器
    private float animationBreakTime;
    private bool canPlayStopAnimationClip;
    public AnimationClip blankAnimationClip;
    public AnimationClip stopAnimationClip;

    //npc移动的协程
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

        //将animatorOverride重新赋值，这个在文档里有
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
            //因为是按时间进行排序，这里可以做一个剪枝
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
        CheckSceneAvaliable(); //加载结束查看npc是否出现在该出现的场景中
        if(!isInitialized)
        {
            isInitialized= true;
            InitNPC();
        }
        sceneLoaded= true;
        //Debug.Log(transform.name + " currentScene: " + currentScene + " targetScene: " + targetScene);
        if (!isFirstLoaded)
        {
            //在场景加载完拿到卡在原地的npc的位置
            //重新制作一个schedule
            currentGridPosition = grid.WorldToCell(transform.position);
            ScheduleDetails newSchedule = new ScheduleDetails(0, 0, 0, 0, currentSeason, targetScene, (Vector2Int)targetGridPosition, stopAnimationClip, isInteractable);
            BuildPath(newSchedule);
            isFirstLoaded = true;
            //游戏第一次开始场景加载会执行，但此时以为targetscene和targetPosition都是当前的场景和位置（看InitNPC函数）
            //所以相当于不动
            //如果在NPC移动中保存，isFirstLoaded变为false，所以重新加载NPC卡住时会执行这行命令
            //如果NPC没动过，也不用担心，因为targetscene和targetPosition依旧是初始位置
        }
    }

    private void OnEndCurrentGame()
    {
        npcMove = false;
        //如果结束游戏时某个NPC恰好在协程中，则直接结束
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

        //计时器部分
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
    /// NPC的主要移动模块
    /// </summary>
    private void Movement() //每0.02s调用一次
    {
        if(!npcMove) //NPC不再移动
        {
            if (movementStepsStack.Count > 0) //如果我有步数要走
            {
                //我在astar里是从终点开始入栈的，所以栈的最上面是起点
                MovementStep step = movementStepsStack.Pop();
                currentScene = step.sceneName;
                CheckSceneAvaliable(); //是否应该出现在这个场景
                nextGridPosition = (Vector3Int)step.gridCoordinate;
                TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);
                MoveToGridPosition(nextGridPosition, stepTime);
            }
            else if(!isMoving && canPlayStopAnimationClip) //多加一个bool的判断是防止人物停止且count<=0，该部分会在fixedupdate里疯狂调用
            {
                StartCoroutine(SetStopAnimation());
            }
        }
    }

    private void MoveToGridPosition(Vector3Int gridPos, TimeSpan stepTime)
    {
        //获得当前正在移动的协程
        npcCoroutine =  StartCoroutine(MoveRoutine(gridPos, stepTime));
    }

    private IEnumerator MoveRoutine(Vector3Int gridPos, TimeSpan stepTime)
    {
        npcMove = true;
        nextWorldPosition = GetWorldPosition(gridPos);
        //还有时间来移动
        if (stepTime > gameTime)
        {
            //用来移动的时间差
            float timeToMove = (float)(stepTime.TotalSeconds - gameTime.TotalSeconds);
            //实际移动距离
            float distance= Vector3.Distance(transform.position, nextWorldPosition);
            
            //实际移动速度
            float speed = Mathf.Max(minSpeed, (distance / timeToMove / Settings.timeThreshold));
            if (speed <=maxSpeed)
            {
                while(Vector3.Distance(transform.position, nextWorldPosition) > Settings.pixelSize)
                {
                    dir = (nextWorldPosition- transform.position).normalized;
                    Vector2 posOffset = new Vector2(dir.x * speed * Time.fixedDeltaTime, dir.y * speed * Time.fixedDeltaTime);
                    rb.MovePosition(rb.position + posOffset);
                    yield return new WaitForFixedUpdate(); //暂停直到下一次FixedUpdate执行
                }
            }
        }
        //如果已经到了就瞬移
        rb.position = nextWorldPosition;
        currentGridPosition = gridPos;
        nextGridPosition = currentGridPosition;
        npcMove = false;
    }

    /// <summary>
    /// 初始化NPC
    /// </summary>
    private void InitNPC()
    {
        targetScene = currentScene;

        //让人物开始时处于网格中心
        currentGridPosition = grid.WorldToCell(transform.position);
        transform.position = new Vector3(currentGridPosition.x + Settings.gridCellSize / 2f, currentGridPosition.y + Settings.gridCellSize / 2f, 0);
        targetGridPosition = currentGridPosition;
    }

    public void BuildPath(ScheduleDetails scheduleDetails) //每一帧调用一次
    {
        movementStepsStack.Clear();
        targetScene = scheduleDetails.targetScene;
        currentScheduleDetails= scheduleDetails;
        targetGridPosition = (Vector3Int)scheduleDetails.targetGridPosition;
        stopAnimationClip = scheduleDetails.animationClipAtStop;
        this.isInteractable = scheduleDetails.isInteractable; //是否可交互

        //在同一场景内移动
        if (scheduleDetails.targetScene == currentScene) 
        {
            AStar.Instance.BuildPath(currentScene, (Vector2Int)currentGridPosition, scheduleDetails.targetGridPosition, movementStepsStack);
        }
        //跨场景移动
        else if(scheduleDetails.targetScene != currentScene)
        {
            SceneRoute sceneRoute = NPCManager.Instance.GetSceneRoute(currentScene, scheduleDetails.targetScene);
            if(sceneRoute!= null)
            {
                //在编辑器中，pathList是为了压栈，所以先进后出，排前面的是终点
                for(int i=0;i<sceneRoute.pathList.Count;i++) 
                {
                    Vector2Int fromPos, toPos;
                    ScenePath scenePath = sceneRoute.pathList[i];
                    //这里以从01_Field走到02_Home为例
                    //1.很显然我需要让npc从当前位置走到01_Field的门处
                    //2.接着从02_Home的门处走到目标位置
                    //对于1.显然执行第一个判断的if
                    //对于2，显然执行第二个判断的else
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
            //更新每一步对应的时间戳
            UpdateTimeOnPath();
        }
    }

    /// <summary>
    /// 将栈内的每一步的时间进行赋值
    /// </summary>
    public void UpdateTimeOnPath()
    {
        MovementStep previousStep = null; //上一步为空，临时变量
        TimeSpan currentGameTime = gameTime;

        foreach(MovementStep step in movementStepsStack)
        {
            if(previousStep == null) //起点上一步为空
                previousStep= step;

            step.hour = currentGameTime.Hours;                                   
            step.minute = currentGameTime.Minutes;
            step.second = currentGameTime.Seconds;

            TimeSpan BetweenStepTime;
            if (IsInDiagonal(previousStep, step)) //如果斜向走
            {
                //游戏内两步间的时间=网格距离/速度/游戏内的时间量度
                BetweenStepTime = new TimeSpan (0, 0, (int)(Settings.gridCellDiagonalSize / normalSpeed / Settings.timeThreshold));
            }
            else
            {
                BetweenStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.timeThreshold));
                //     1/2f/0.002=250s
            }
            //每一步走的时间
            currentGameTime = currentGameTime.Add(BetweenStepTime);
            previousStep= step;
        }
    }

    /// <summary>
    /// 判断两步是否为斜着
    /// </summary>
    /// <param name="currentStep"></param>
    /// <param name="previosStep"></param>
    /// <returns></returns>
    private bool IsInDiagonal(MovementStep currentStep, MovementStep previosStep)
    {
        return (currentStep.gridCoordinate.x != previosStep.gridCoordinate.x) && (currentStep.gridCoordinate.y != previosStep.gridCoordinate.y);
    }

    /// <summary>
    /// 网格坐标返回世界坐标
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
            anim.SetBool("ForceExit", true); //一旦开始走路，强制推出现阶段的动画
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
        animationBreakTime = Settings.animationBreakTime; //还原计时器
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

        //重新加载NPC，已经初始化
        isInitialized= true; 
    }
}
