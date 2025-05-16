namespace TicTacToeOnline.Ui.Views
{
    using TicTacToeOnline.Gameplay;
    using UnityEngine;
    using UnityEngine.UI;

    public class SettingsView : BaseView
    {
        [SerializeField]
        private Slider musicSlider = null;

        [SerializeField]
        private Slider effectsSlider = null;

        #region Unity Methods

        private void OnEnable()
        {
            musicSlider.value = PlayerPrefs.HasKey(GameManager.MUSIC_VOLUME_PREFS_KEY) ? PlayerPrefs.GetFloat(GameManager.MUSIC_VOLUME_PREFS_KEY) : 1.0f;
            effectsSlider.value = PlayerPrefs.HasKey(GameManager.EFFECTS_VOLUME_PREFS_KEY) ? PlayerPrefs.GetFloat(GameManager.EFFECTS_VOLUME_PREFS_KEY) : 1.0f;
            musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
            effectsSlider.onValueChanged.AddListener(OnEffectsSliderValueChanged);
        }

        private void OnDisable()
        {
            musicSlider.onValueChanged.RemoveListener(OnMusicSliderValueChanged);
            effectsSlider.onValueChanged.RemoveListener(OnEffectsSliderValueChanged);
        }

        #endregion

        private void OnMusicSliderValueChanged(float volume)
        {
            GameManager.Instance.UpdateMusicVolume(volume);
        }

        private void OnEffectsSliderValueChanged(float volume)
        {
            GameManager.Instance.UpdateEffectsVolume(volume);
        }

        protected override void OnGoBackActionPerformed()
        {
            base.OnGoBackActionPerformed();
            viewManager.RemoveView<SettingsView>();
            viewManager.DisplayView<MainMenuView>();
        }
    }
}

