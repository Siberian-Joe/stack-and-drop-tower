namespace Game.UI.BottomBar.Contracts
{
    public readonly struct PlacementRuleResult
    {
        public bool IsSuccess { get; }
        public string FailureLocalizationKey { get; }

        private PlacementRuleResult(bool isSuccess, string failureLocalizationKey)
        {
            IsSuccess = isSuccess;
            FailureLocalizationKey = failureLocalizationKey;
        }

        public static PlacementRuleResult Success() => new(true, null);

        public static PlacementRuleResult Fail(string failureLocalizationKey) =>
            new(false, failureLocalizationKey);
    }
}