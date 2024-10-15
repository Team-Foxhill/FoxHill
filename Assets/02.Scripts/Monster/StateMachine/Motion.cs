//using System;
//using UnityEngine;

//namespace FoxHill.Monster.AI
//{
//    public class Motion : Node
//    {
//        public Motion(MonsterBehaviourTree tree, SpriteData[] spritesData, bool isLoop, int fps) : base(tree)
//        {
//            spriteRenderer = tree.GetComponent<SpriteRenderer>();
//            this.spritesData = spritesData;
//            this.isLoop = isLoop;
//            this.fps = fps;
//        }

//        public class SpriteData
//        {
//            public Sprite sprite;
//            public int frameCount;
//            public Action eventFunction;
//        }
//        protected SpriteData[] spritesData;
//        protected SpriteRenderer spriteRenderer;
//        protected bool isLoop;
//        protected int fps;
//        protected int currentFrameIndex;
//        protected float elapsedTime;
//        protected int elapsedFrames;


//        public override Result Invoke()
//        {
//            Result result = Result.Failure;
//            var spriteData = spritesData[currentFrameIndex];
//            //실행할 이벤트 혹은 로직.


//            if (elapsedTime < spriteData.frameCount / fps)
//            {
//                result = Result.Running;
//            }
//            else
//            {

//            }


//            elapsedTime += Time.deltaTime;
//            elapsedFrames = (int)elapsedTime * fps;

//            if (currentFrameIndex >= spritesData.Length)
//            {
//                if(!isLoop)
//                {
//                    return Result.Success;
//                }
//                currentFrameIndex = 0;
//            }

//            return result;

//        }
//    }
//}
