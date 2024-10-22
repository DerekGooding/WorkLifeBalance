using WorkLifeBalance.Interfaces;
using WorkLifeBalance.Services.Feature;

namespace WorkLifeBalance.Services;

public class FeaturesService(Func<Type, FeatureBase> featureFactory, AppTimer appTimer) : IFeaturesServices
{
    private readonly Func<Type, FeatureBase> _featureFactory = featureFactory;
    private readonly AppTimer _appTimer = appTimer;

    public void AddFeature<TFeature>() where TFeature : FeatureBase
    {
        FeatureBase feature = _featureFactory.Invoke(typeof(TFeature));
        _appTimer.Subscribe(feature.AddFeature());
    }

    public void RemoveFeature<TFeature>() where TFeature : FeatureBase
    {
        FeatureBase feature = _featureFactory.Invoke(typeof(TFeature));
        _appTimer.UnSubscribe(feature.RemoveFeature());
    }
}
