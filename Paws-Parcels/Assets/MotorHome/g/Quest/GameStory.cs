using System.Collections;
using MotorHome.Scripts.Saves;
using R3;
using UnityEngine;
using Zenject;

namespace MotorHome.Scripts
{
    public class GameStory : MonoBehaviour
    {
        //[SerializeField] private NPCInteractable _npcInteractableCat;
        [SerializeField] private NPCInteractable _npcInteractableOlen;
        
        private CompassUI _compassUI;
        private GasSystem _gasSystem;
        private MoneySystem _moneySystem;
        [Inject]
        private MusicManager _musicManager;
        [Inject]
        private QuestTargets _questTargets;
        [Inject]
        private QuestsSystem _questsSystem;
        [Inject]
        private CoreSaver _coreSaver;
        
        [Inject]
        private void Construct(CoreUI coreUI, GasSystem gasSystem, MoneySystem moneySystem)
        {
            _compassUI = coreUI.CompassUI;
            _gasSystem = gasSystem;
            _moneySystem = moneySystem;
        }
        
        //после загрузки сохранений
        private void Start()
        {
            StartGameStory();
            
        }
        
        private void StartGameStory()
        {
            StartCoroutine(F());
            _gasSystem.SetGas(0.1f);
            //игрок подъекхал к заправке(геймплей начинается) игрок вышел из машины
            //монолог игрока, что он хочет заправиться, но нет денег и есму мешют коробки
            //квест убрать коробки
            /*_npcInteractableCat.CanStart().Subscribe(_ =>
            {
                _npcInteractableBear.CanStart();
            });*/
            //над коробкой подсказка
            //отложил 2 коробки
            //берет 3 коробку, а там котенок(кадцена)
            //гг говорит с котенком(диалог)
            //квест отнести коробку в машину
            //он берет корбку с котиком и относит в машину м подсказками
            //монлог - что ему делать с котиком
            //потом идет к заправке и говорит с заправщиком
            //игрок получает компас удачи
            _compassUI.ActivateCompassUI();
            _compassUI.SetTarget(_questTargets.GetQuestTarget(QuestEnum.FirstQuest));
            
            //заправляется в залог за то, что отвезет эти коробки учителю
            //даем задачу довезти коробки к учителю
            //нужно взять корбки 3 коробки и кладет на крышу
            //обязательно закрыть дверь(нужна проверка)
            //сесть в машину
            //обучение с компасом
            //доехать до школы (вечереет)

            //ночь

            //отдать коробки учителю по одной(начать диалог)(задача выполнена)
            //после разговора новый квест вернуться к заправщику с деталями шинами за плату
            //садится в машину и едет по компасу до заправщика
            //диалог с заправщиком(таск выполнен)
            //новый таск доставить бабушке медикаментов и холст для рисования.
            //игрок кладет всё в машину
            //едет до бабушки(Светлеет)

            //день
            //1 день окончен
            //начался дождь

            //отдает бабушке вещи(квест закончен)
            //квест довезти бабушке булочки
            //едет в кондитерскую
            //булочниуе нужны яйца(+1 квест)
            //едет на ферму

            //дождь закончился

            //говорит с фермеренком и он ему дает продукты
            //едет в кондитерскую и по дороге монолог, что заканчивается бензин (вечереет)

            //ночь

            //квест закончен с кондитерской и она просит его подождать до утра
            //когда садится в машину монолог, что бензин почти на нуле
            //едет на заправку
            //едет обратно

            //получает булки и едет к бабушке
            //квест закончен с булочками
            //новый квест: доставка книг в школу
            //едет до школы
            //отдает книги(квест закончен)
            //диалог, что начался пожар
            //новый квест спасти олененка
            //едет до фермы(вечереет и начался дождь)

            //ночь
            //дождь

            //подъезжает к ферме подквест объехать все объекты???? или не делать подквест? и если делать, то
            //тогда нужно вывести его в UI, как в dying light 2?
            //отключить возможность выходить
            //объезжает все объекты и подъезжает к фермеренку
            //сажает его в машину внутрь
            //отвозит его к бабушке
            //диалог с бабушкой(квест спасти олененка выполнен)
            //финальная кадсцена
        }
        
        private IEnumerator F()
        {
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            yield return null;
            
            //_musicManager.SetRainy();
            _compassUI.ActivateCompassUI();

            _questsSystem.NewQuest(QuestEnum.FirstQuest);
            _questsSystem.NewQuest(QuestEnum.SecondQuest);
            _npcInteractableOlen.End.Subscribe(_ =>
            {
                _questsSystem.CompleteQuest(QuestEnum.FirstQuest);
            });
            
            bool d = true;
            while (true)
            {
                //_moneySystem.AddMoney(Random.Range(1, 100));
                //_gasSystem.SetGas(Random.Range(0f, 1f));
                
                
                if (d)
                {
                    _musicManager.SetSunny();
                }
                else
                {
                    _musicManager.SetRainy();
                }

                d = !d;
                yield return new WaitForSeconds(40f);
                _coreSaver.Save("Auto Save");
            }
        }
    }
}