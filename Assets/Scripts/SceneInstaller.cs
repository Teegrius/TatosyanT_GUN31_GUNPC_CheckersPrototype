using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private GameObject _whiteUnitPrefab;
    [SerializeField] private GameObject _blackUnitPrefab;
    [SerializeField] private Battlefield _battlefield;
    [SerializeField] private TurnPanel _turnPanel;
    [SerializeField] private RestartPanel _restartPanel;

    public override void InstallBindings()
    {
        // Battlefield
        Container.Bind<Battlefield>().FromInstance(_battlefield).AsSingle();

        // Controllers
        Container.Bind<BattleController>().AsSingle().NonLazy();
        Container.Bind<PlayerController>().AsSingle().NonLazy();
        Container.Bind<InputController>().AsSingle().NonLazy();
        Container.Bind<UIController>().AsSingle().NonLazy();

        // UI
        Container.Bind<TurnPanel>().FromInstance(_turnPanel).AsSingle();
        Container.Bind<RestartPanel>().FromInstance(_restartPanel).AsSingle();

        // Commands
        Container.Bind<IGameplayCommand>().To<SelectCommand>().AsSingle();

        // Factories
        Container.BindFactory<Cell, Cell.Factory>().FromComponentInNewPrefab(_cellPrefab);
        Container.BindFactory<Team, Unit, Unit.Factory>().FromMethod(CreateUnit);
    }

    private Unit CreateUnit(DiContainer subContainer, Team team)
    {
        GameObject prefab = team == Team.White ? _whiteUnitPrefab : _blackUnitPrefab;
        return subContainer.InstantiatePrefab(prefab).GetComponent<Unit>();
    }
}