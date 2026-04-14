// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using SongLib;
// using SongLib.Core.Singleton;
// using UnityEngine;
//
// public class ChapterManager : MonoBehaviourSingleton<ChapterManager>, IGameInitializer
// {
//     public List<ChapterDefinition> chapterDefinitions;
//     public Dictionary<string, ChapterDefinition> chapterMap = new Dictionary<string, ChapterDefinition>();
//     
//     private string currentChapterAddress;
//     private ConstellationData currentAnswerData;
//     
//     public void Initialize(Action onCompleted)
//     {
//         foreach (var chapterDefinition in chapterDefinitions)
//         {
//             chapterMap.Add(chapterDefinition.chapterId, chapterDefinition);
//         }
//
//         SetupPuzzle();
//         
//         onCompleted?.Invoke();
//     }
//     
//     public void SetCurrentChapterAddress(string address)
//     {
//         currentChapterAddress = ScriptableObjectAddressManager.GetChapterAddress(address);
//     }
//
//     public async Task LoadPuzzle()
//     {
//         Debug.Log("정답 데이터 비동기 로딩 시작...");
//
//         // 1. SongLib의 어드레서블 매니저를 통해 SO 데이터를 비동기로 가져옴
//         currentAnswerData = await AddressableManager.LoadAssetAsync<ConstellationData>(currentChapterAddress);
//
//         // 2. 로딩 성공 시 매니저에 주입
//         if (currentAnswerData != null)
//         {
//             Debug.Log($"로딩 성공! 이번 퍼즐: {currentAnswerData.constellationName}");
//         }
//     }
//
//
//     private void SetupPuzzle()
//     {
//         ConstellationManager.Instance.SetCurrentPuzzle(currentAnswerData);
//     }
//     
//     public string GetNextChapterKey(string chapterId)
//     {
//         int index = chapterDefinitions.FindIndex(c => c.chapterId == chapterId);
//
//         return index < chapterDefinitions.Count - 1 ? chapterDefinitions[index + 1].chapterId : null;
//     }
//     
//     private void OnDestroy()
//     {
//         AddressableManager.ReleaseAsset(currentChapterAddress);
//     }
//
// }
