using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WindowsTweaks
{
    /// <summary>
    /// Движок для применения системных твиков Windows
    /// Добавлена поддержка отмены твиков + 13 новых твиков
    /// </summary>
    public partial class TweakEngine
    {
        private readonly HashSet<string> enabledTweaks;
        private readonly Dictionary<string, TweakAction> tweakActions;
        private readonly List<string> appliedTweaks;
        private readonly List<string> failedTweaks;
        private readonly List<string> lastOperationTweaks;
        private readonly string statePath;

        public TweakEngine()
        {
            enabledTweaks = new HashSet<string>();
            appliedTweaks = new List<string>();
            failedTweaks = new List<string>();
            tweakActions = InitializeTweakActions();

            statePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "WindowsTweaks",
                "applied_tweaks.txt"
            );

            LoadAppliedTweaksState();

            lastOperationTweaks = new List<string>();

            // Синхронизируем enabledTweaks с уже применёнными твиками.
            // Без этого при старте enabledTweaks пуст, и кнопка "Отменить"
            // считает ВСЕ применённые твики "отключёнными" — и предлагает их отменить.
            foreach (var tweak in appliedTweaks)
                enabledTweaks.Add(tweak);
        }

        public void EnableTweak(string tweakKey)
        {
            enabledTweaks.Add(tweakKey);
        }

        public void DisableTweak(string tweakKey)
        {
            enabledTweaks.Remove(tweakKey);
        }

        public bool IsTweakApplied(string tweakKey)
        {
            return appliedTweaks.Contains(tweakKey);
        }

        public bool IsTweakEnabled(string tweakKey)
        {
            return enabledTweaks.Contains(tweakKey);
        }

        public List<string> GetAppliedTweaks()
        {
            return new List<string>(appliedTweaks);
        }

        /// <summary>
        /// Возвращает список твиков, которые включены (галочка стоит), но ещё не применены к системе.
        /// Используется кнопкой "Применить".
        /// </summary>
        public List<string> GetEnabledButNotAppliedTweaks()
        {
            return enabledTweaks
                .Where(t => !appliedTweaks.Contains(t))
                .ToList();
        }

        /// <summary>
        /// Применяет конкретный список твиков (по кнопке "Применить").
        /// </summary>
        public async Task ApplySelectedTweaksAsync(List<string> tweakKeys)
        {
            failedTweaks.Clear();
            lastOperationTweaks.Clear();

            await Task.Run(() =>
            {
                foreach (var tweakKey in tweakKeys)
                {
                    if (tweakActions.ContainsKey(tweakKey))
                    {
                        try
                        {
                            tweakActions[tweakKey].Apply();
                            if (!appliedTweaks.Contains(tweakKey))
                                appliedTweaks.Add(tweakKey);
                            lastOperationTweaks.Add(tweakKey);
                            Debug.WriteLine($"✓ Применён твик: {tweakKey}");
                        }
                        catch (UnauthorizedAccessException)
                        {
                            failedTweaks.Add($"{tweakKey} (требуются права администратора)");
                        }
                        catch (Exception ex)
                        {
                            failedTweaks.Add($"{tweakKey} ({ex.Message})");
                        }
                    }
                }
            });

            SaveAppliedTweaksState();
            LogResults();
        }

        public async Task RevertSelectedTweaksAsync(IEnumerable<string> tweaksToRevert)
        {
            failedTweaks.Clear();

            await Task.Run(() =>
            {
                foreach (var tweakKey in tweaksToRevert)
                {
                    if (tweakActions.ContainsKey(tweakKey))
                    {
                        try
                        {
                            tweakActions[tweakKey].Revert();
                            appliedTweaks.Remove(tweakKey);
                            Debug.WriteLine($"✓ Успешно отменен твик: {tweakKey}");
                        }
                        catch (Exception ex)
                        {
                            failedTweaks.Add($"{tweakKey} ({ex.Message})");
                        }
                    }
                }
            });

            SaveAppliedTweaksState();
            LogResults();
        }

        private void LoadAppliedTweaksState()
        {
            try
            {
                if (File.Exists(statePath))
                {
                    var lines = File.ReadAllLines(statePath);
                    appliedTweaks.Clear();
                    appliedTweaks.AddRange(lines.Where(l => !string.IsNullOrWhiteSpace(l)));
                }
            }
            catch { }
        }

        private void SaveAppliedTweaksState()
        {
            try
            {
                string directory = Path.GetDirectoryName(statePath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                File.WriteAllLines(statePath, appliedTweaks);
            }
            catch { }
        }

        private string GetApplyResults()
        {
            var result = new System.Text.StringBuilder();
            result.AppendLine("◆  РЕЗУЛЬТАТЫ ПРИМЕНЕНИЯ ТВИКОВ");
            result.AppendLine("────────────────────────────────────────────");
            result.AppendLine();

            if (lastOperationTweaks.Count > 0)
            {
                result.AppendLine($"✓  Успешно применено:  {lastOperationTweaks.Count}");
                foreach (var tweak in lastOperationTweaks)
                {
                    if (tweakActions.ContainsKey(tweak))
                        result.AppendLine($"   •  {tweakActions[tweak].Description}");
                }
                result.AppendLine();
            }

            if (failedTweaks.Count > 0)
            {
                result.AppendLine($"✗  Ошибок:  {failedTweaks.Count}");
                foreach (var tweak in failedTweaks)
                    result.AppendLine($"   •  {tweak}");
                result.AppendLine();
            }

            result.AppendLine("────────────────────────────────────────────");
            return result.ToString();
        }

        private void LogResults()
        {
            try
            {
                string logPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "WindowsTweaks", "Logs");
                Directory.CreateDirectory(logPath);
                string logFile = Path.Combine(logPath, $"tweaks_{DateTime.Now:yyyyMMdd_HHmmss}.log");
                File.WriteAllText(logFile, GetApplyResults());
            }
            catch { }
        }

    }
}
