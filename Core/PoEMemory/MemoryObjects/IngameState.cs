using System;
using GameOffsets;
using Shared.Interfaces;
using Shared.Enums;
using Shared.Helpers;

namespace PoEMemory
{
    public class IngameState : RemoteMemoryObject
    {
        private CachedValue<Camera> _camera;
        private CachedValue<IngameUIElements> _ingameUi;
        private CachedValue<ServerData> _serverData;
        private CachedValue<IngameData> _ingameData;
        private CachedValue<Element> _UIRoot;
        private CachedValue<Element> _UIHover;
        private CachedValue<float> _UIHoverX;
        private CachedValue<float> _UIHoverY;
        private CachedValue<Element> _UIHoverTooltip;
        private CachedValue<float> _CurrentUElementPosX;
        private CachedValue<float> _CurrentUElementPosY;
        private CachedValue<DiagnosticElement> _LatencyRectangle;
        private CachedValue<DiagnosticElement> _FPSRectangle;
        private CachedValue<DiagnosticElement> _FrameTimeRectangle;
        private CachedValue<float> _TimeInGameF;
        private CachedValue<DiagnosticInfoType> _DiagnosticInfoType;
        private CachedValue<IngameStateOffsets> _ingameState;
        private CachedValue<EntityLabelMapOffsets> _EntityLabelMap;
        private CachedValue<Element> _mouseActions;


        public IngameState() {
            _ingameState = new FrameCache<IngameStateOffsets>(() => M.Read<IngameStateOffsets>(Address /*+M.offsets.IgsOffsetDelta*/));
            _camera = new AreaCache<Camera>(
                () => GetObject<Camera>(Address + /*0x1258*/Extensions.GetOffset<IngameStateOffsets>(nameof(IngameStateOffsets.Camera))));
            _ingameData = new AreaCache<IngameData>(() => GetObject<IngameData>(_ingameState.Value.Data));
            _serverData = new AreaCache<ServerData>(() => GetObject<ServerData>(_ingameState.Value.ServerData));
            _ingameUi = new AreaCache<IngameUIElements>(() => GetObject<IngameUIElements>(_ingameState.Value.IngameUi));
            _UIRoot = new AreaCache<Element>(() => GetObject<Element>(_ingameState.Value.UIRoot));
            _UIHover = new FrameCache<Element>(() => GetObject<Element>(_ingameState.Value.UIHover));
            _UIHoverX = new FrameCache<float>(() => _ingameState.Value.UIHoverX);
            _UIHoverY = new FrameCache<float>(() => _ingameState.Value.UIHoverY);
            _UIHoverTooltip = new FrameCache<Element>(() => GetObject<Element>(_ingameState.Value.UIHoverTooltip));
            _CurrentUElementPosX = new FrameCache<float>(() => _ingameState.Value.CurentUElementPosX);
            _CurrentUElementPosY = new FrameCache<float>(() => _ingameState.Value.CurentUElementPosY);
            _DiagnosticInfoType = new FrameCache<DiagnosticInfoType>(() => (DiagnosticInfoType) _ingameState.Value.DiagnosticInfoType);
            _LatencyRectangle = new AreaCache<DiagnosticElement>(
                () => GetObject<DiagnosticElement>(
                    Address + Extensions.GetOffset<IngameStateOffsets>(nameof(IngameStateOffsets.LatencyRectangle))));
            _FrameTimeRectangle = new AreaCache<DiagnosticElement>(
                () => GetObject<DiagnosticElement>(Address + /*0x1628*/
                                                   +Extensions.GetOffset<IngameStateOffsets>(
                                                       nameof(IngameStateOffsets.FrameTimeRectangle))));
            _FPSRectangle = new AreaCache<DiagnosticElement>(
                () => GetObject<DiagnosticElement>(Address /*0x1870*/ +
                                                   Extensions.GetOffset<IngameStateOffsets>(nameof(IngameStateOffsets.FPSRectangle))));
            _TimeInGameF = new FrameCache<float>(() => _ingameState.Value.TimeInGameF);
            _EntityLabelMap = new AreaCache<EntityLabelMapOffsets>(() => M.Read<EntityLabelMapOffsets>(_ingameState.Value.EntityLabelMap));
        }

        public Camera Camera => _camera.Value;
        public IngameData Data => _ingameData.Value;
        public bool InGame => ServerData.IsInGame;
        public ServerData ServerData => _serverData.Value;
        public IngameUIElements IngameUi => _ingameUi.Value;

        public Element UIRoot => _UIRoot.Value;
        public Element UIHover => _UIHover.Value;
        public float UIHoverX => _UIHoverX.Value;
        public float UIHoverY => _UIHoverY.Value;
        public Element UIHoverTooltip => _UIHoverTooltip.Value;

        public float CurentUElementPosX => _CurrentUElementPosX.Value;
        public float CurentUElementPosY => _CurrentUElementPosY.Value;

        public long EntityLabelMap => _EntityLabelMap.Value.EntityLabelMap;
        public DiagnosticInfoType DiagnosticInfoType => _DiagnosticInfoType.Value;
        public DiagnosticElement LatencyRectangle => _LatencyRectangle.Value;

        public DiagnosticElement FrameTimeRectangle => _FrameTimeRectangle.Value;
        public DiagnosticElement FPSRectangle => _FPSRectangle.Value;
        public float CurLatency => LatencyRectangle.CurrValue;
        public float CurFrameTime => FrameTimeRectangle.CurrValue;
        public float CurFps => FPSRectangle.CurrValue;
        public TimeSpan TimeInGame => TimeSpan.FromSeconds(_ingameState.Value.TimeInGame);
        public float TimeInGameF => _TimeInGameF.Value;

        public void UpdateData() {
            _ingameData.ForceUpdate();
        }
    }
}