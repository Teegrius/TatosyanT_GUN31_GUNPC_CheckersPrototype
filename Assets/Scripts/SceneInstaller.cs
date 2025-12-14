using UnityEngine;
using Zenject;

public class SceneInstaller : MonoInstaller
{
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private GameObject _whiteUnitPrefab;
    [SerializeField] private GameObject _blackUnitPrefab;
    [SerializeField] private Battlefield _battlefield;

    public override void InstallBindings()
    {
        Container.Bind<Battlefield>().FromInstance(_battlefield).AsSingle();
        Container.Bind<BattleController>().AsSingle().NonLazy();
        Container.Bind<PlayerController>().AsSingle().NonLazy();

        Container.BindFactory<Cell, Cell.Factory>().FromComponentInNewPrefab(_cellPrefab);
        Container.BindFactory<Team, Unit, Unit.Factory>().FromMethod(CreateUnit);
    }

    private Unit CreateUnit(DiContainer subContainer, Team team)
    {
        GameObject prefab = team == Team.White ? _whiteUnitPrefab : _blackUnitPrefab;
        return subContainer.InstantiatePrefab(prefab).GetComponent<Unit>();
    }
}