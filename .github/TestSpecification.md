# テスト仕様書

対象プロジェクト：`tests\SignalProcessingToolApp.Tests`
テストフレームワーク：xUnit
テスト件数：66 件

## 1. Applications 層

### 1.1 Configs

#### GainConfigTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| Constructor_SetsProperties | 正常系 | `IsEnabled = true`、`GainDb = -6.0` で生成 | `IsEnabled` と `GainDb` が指定値と一致する |

#### SignalProcessingConfigTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| Constructor_SetsGainConfig | 正常系 | `GainConfig` を生成して渡す | `GainConfig` が同一インスタンスとして保持される |
| Constructor_WithNullGainConfig_ThrowsArgumentNullException | 異常系 | `null` を渡す | `ArgumentNullException` がスローされる |

### 1.2 DTOs

#### SignalExecutionRequestTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| Constructor_SetsProperties | 正常系 | 正弦波、各パラメータを指定して生成 | `WaveformType`、各パラメータ、処理設定が指定値と一致する |

### 1.3 Pipelines

#### FrameProcessorTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| Constructor_InitializesStartIndexToZero | 初期状態 | 生成直後 | `CurrentStartIndex` が 0 |
| AdvanceFrame_IncreasesStartIndexByFrameSize | 正常系 | フレームサイズ 8 を進める | `CurrentStartIndex` が 8 |
| HasNextFrame_ReturnsFalse_WhenStartIndexReachesDataSize | 境界値 | 開始位置 8 | データサイズ 16 では `true`、8 では `false` |
| Reset_SetsStartIndexToZero | 正常系 | 開始位置 8 の状態でリセット | `CurrentStartIndex` が 0 |

#### SignalProcessorTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| Process_WhenGainDisabled_CopiesInputToOutput | 正常系 | ゲイン無効 | 出力が入力と一致する |
| Process_WhenGainEnabled_AppliesGain | 正常系 | ゲイン -20 dB 有効 | 出力が入力の 0.1 倍になる |
| Constructor_WithNullGainProcessor_ThrowsArgumentNullException | 異常系 | `null` を渡す | `ArgumentNullException` がスローされる |

#### WaveformGeneratorTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| GenerateWaveform_UnitImpulse_GeneratesImpulseAtStart | 正常系 | 単位インパルス指定 | 先頭のみ 1.0、他は 0.0 |
| GenerateWaveform_SineWave_GeneratesSineWave | 正常系 | 正弦波指定 | 各サンプルが `Math.Sin` の値と一致する（小数点以下 10 桁） |
| GenerateWaveform_SineWaveWithNullParameter_ThrowsArgumentNullException | 異常系 | 正弦波指定かつパラメータ `null` | `ArgumentNullException` がスローされる |
| GenerateWaveform_InvalidWaveformType_ThrowsArgumentException | 異常系 | 定義外の `WaveformType` 値 | `ArgumentException` がスローされる |

#### SignalProcessingPipelineTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| ExecuteBatch_GeneratesInputAndProcessedSignal | 正常系 | 単位インパルス、ゲイン無効 | 入力は `[1,0,0,0]`、処理後は入力と一致する |
| ExecuteFrame_AdvancesStartIndexByFrameSize | 正常系 | フレームサイズ 2 で 1 フレーム実行 | `CurrentStartIndex` が 2、次フレームが存在する |
| Reset_SetsStartIndexToZero | 正常系 | 1 フレーム実行後にリセット | `CurrentStartIndex` が 0 |

### 1.4 Sessions

#### SignalProcessingSessionTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| ExecuteBatch_ReturnsFullDataSize | 正常系 | データサイズ 8 | 入力・処理後とも長さ 8、先頭値 1.0 |
| ExecuteNextFrame_ReturnsAccumulatedData | 正常系 | フレームサイズ 4、データサイズ 8 で 1 回実行 | 表示データの長さが 4、未完了、初期化済み |
| ExecuteNextFrame_ReturnsCompleted_WhenAllFramesProcessed | 境界値 | 2 回実行（データサイズ到達） | 表示データの長さが 8、完了フラグが `true` |
| Reset_ClearsInitializedState | 正常系 | 1 回実行後にリセット | 初期化済みフラグが `false` |

## 2. Domains 層

### 2.1 Parameters

#### CommonParameterTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| Constructor_SetsProperties | 正常系 | 44100、8、16 | 各プロパティが指定値と一致する |
| Constructor_WithNonPositiveSamplingFrequency_ThrowsArgumentException | 異常系 | サンプリング周波数 0 | `ArgumentException`、`ParamName` は `samplingFrequency` |
| Constructor_WithNonPositiveFrameSize_ThrowsArgumentException | 異常系 | フレームサイズ 0 | `ArgumentException`、`ParamName` は `frameSize` |
| Constructor_WithNonPositiveDataSize_ThrowsArgumentException | 異常系 | データサイズ 0 | `ArgumentException`、`ParamName` は `dataSize` |
| Constructor_WhenFrameSizeExceedsDataSize_ThrowsArgumentException | 異常系 | フレームサイズ 16、データサイズ 8 | `ArgumentException`、`ParamName` は `frameSize` |

### 2.2 SignalProcessing

#### GainProcessorTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| Apply_WithZeroDecibel_KeepsInputUnchanged | 境界値 | 0 dB | 出力が入力と一致する |
| Apply_WithMinusTwentyDecibel_ScalesByTenth | 正常系 | -20 dB | 出力が入力の 0.1 倍になる |
| Apply_WithMinusInfinityDecibel_ProducesZeros | 境界値 | -200 dB | 出力がすべて 0.0 |
| Apply_WithEmptyInput_ThrowsArgumentException | 異常系 | 空の入力配列 | `ArgumentException`、`ParamName` は `input` |
| Apply_WithEmptyOutput_ThrowsArgumentException | 異常系 | 空の出力配列 | `ArgumentException`、`ParamName` は `output` |
| Apply_WhenOutputShorterThanInput_ThrowsArgumentException | 異常系 | 出力長が入力より短い | `ArgumentException`、`ParamName` は `output` |

### 2.3 WaveformGeneration

#### SineWaveGeneratorTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| Generate_FillsOutputWithSineWave | 正常系 | 周波数 1000 Hz、サンプリング 8000 Hz | 各サンプルが `Math.Sin` の値と一致する |
| Generate_WithStartIndex_OffsetsPhase | 正常系 | 開始インデックス 1 | 位相がずれた値になる |
| Generate_WithEmptyOutput_ThrowsArgumentException | 異常系 | 空の出力配列 | `ArgumentException`、`ParamName` は `output` |

#### SineWaveParameterTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| Constructor_WithValidValues_SetsFrequency | 正常系 | 1000 Hz、44100 Hz | `Frequency` が 1000 |
| Constructor_WithNonPositiveFrequency_ThrowsArgumentException | 異常系 | 周波数 0 | `ArgumentException`、`ParamName` は `frequency` |
| Constructor_WithNonPositiveSamplingFrequency_ThrowsArgumentException | 異常系 | サンプリング周波数 0 | `ArgumentException`、`ParamName` は `samplingFrequency` |
| Constructor_WhenFrequencyReachesNyquist_ThrowsArgumentException | 境界値 | 22050 Hz、44100 Hz（ナイキスト周波数と同値） | `ArgumentException`、`ParamName` は `frequency` |

#### UnitImpulseGeneratorTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| Generate_WithStartIndexZero_PlacesImpulseAtFirstSample | 正常系 | 開始インデックス 0 | `[1,0,0,0]` |
| Generate_WithNonZeroStartIndex_ProducesNoImpulse | 境界値 | 開始インデックス 4、出力長 4 | 全要素が 0.0 |
| Generate_WithEmptyOutput_ThrowsArgumentException | 異常系 | 空の出力配列 | `ArgumentException`、`ParamName` は `output` |

## 3. Presentations 層

### 3.1 Factories

#### SignalExecutionRequestFactoryTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| Create_WithDefaultSineWaveViewModels_CreatesSineWaveRequest | 正常系 | 既定値の ViewModel | 正弦波、44100 Hz、フレーム 8、データ 32、周波数 1000 Hz、ゲイン無効 |
| Create_WithImpulseViewModel_CreatesUnitImpulseRequestWithoutSineWaveParameter | 正常系 | 波形種別をインパルスに設定 | `UnitImpulse`、`SineWaveParameter` が `null` |
| Create_WithGainEnabled_CreatesGainConfigFromSelectedValue | 正常系 | ゲイン有効、選択値インデックス 2 | ゲイン有効、-6 dB |

### 3.2 States

#### ProcessStateManagerTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| Initial_StatusIsReadyAndControlsAreEnabled | 初期状態 | 生成直後 | 「準備完了」、パラメータ・一括処理が有効 |
| UpdateFrameState_WhenNotCompleted_DisablesParametersAndBatchProcess | 正常系 | フレーム処理中 | 「フレーム処理中」、パラメータ・一括処理が無効 |
| UpdateFrameState_WhenCompleted_SetsCompletedState | 正常系 | フレーム処理完了 | 「処理完了」、パラメータ・一括処理が有効 |
| CompleteBatch_SetsCompletedState | 正常系 | 一括処理完了 | 「処理完了」 |
| Reset_ReturnsToInitialState | 正常系 | 処理完了後にリセット | 「準備完了」 |

### 3.3 ViewModels

#### MainWindowViewModelTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| Initial_StatusIsReadyAndDataSizeIsDefault | 初期状態 | 生成直後 | 「準備完了」、データサイズ 32、パラメータ有効 |
| BatchProcessCommand_UpdatesPlotAndCompletesStatus | 正常系 | 一括処理コマンド実行 | 「処理完了」、入出力グラフデータの長さが 32 |
| FrameProcessCommand_UpdatesPlotAndFrameProcessingStatus | 正常系 | フレーム処理コマンド実行 | 「フレーム処理中」、グラフデータの長さが 8、一括処理が無効 |
| ResetCommand_ClearsPlotAndReturnsToInitialStatus | 正常系 | フレーム処理後にリセット | 「準備完了」、グラフデータが `null` |
| FrameProcessCommand_WhenParametersInvalid_RaisesErrorAndResets | 異常系 | フレームサイズがデータサイズを超える | `ErrorOccurred` が発生し「入力値のエラー」で始まるメッセージ、「準備完了」に戻る |

#### SubViewModels/CommonParameterViewModelTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| Initial_HasDefaultIndexes | 初期状態 | 生成直後 | サンプリング 0、フレーム 0、データ 2 |
| Initial_ContainsSelectableValues | 正常系 | 生成直後 | 各コンボボックスの選択肢が仕様どおり |

#### SubViewModels/InputSignalViewModelTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| Initial_IsSineWaveSelected | 初期状態 | 生成直後 | 正弦波が選択、周波数の初期値は 1000 Hz |
| WaveformTypeIndex_WhenImpulse_SelectsImpulse | 正常系 | インデックスを 1 に設定 | インパルスが選択される |
| Initial_ContainsSelectableValues | 正常系 | 生成直後 | 波形種別と周波数の選択肢が仕様どおり |

#### SubViewModels/PlotViewModelTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| Initial_HasNoData | 初期状態 | 生成直後 | 入力・処理後データがともに `null` |
| UpdateData_SetsInputAndProcessedData | 正常系 | 配列を渡して更新 | 同一参照が保持される |
| Clear_RemovesData | 正常系 | データ設定後にクリア | 入力・処理後データがともに `null` |

#### SubViewModels/SignalProcessingViewModelTests

| テストメソッド | 観点 | 前提条件 | 期待結果 |
|---|---|---|---|
| Initial_GainIsDisabledWithFirstValue | 初期状態 | 生成直後 | ゲイン無効、インデックス 0 |
| Initial_ContainsSelectableGainValues | 正常系 | 生成直後 | 0、-3、-6、-9、-12、-15、-18、-200 dB |

## 4. テスト対象外

| 対象 | 理由 |
|---|---|
| `App.xaml.cs` | DI のコンポジションルートのみで、ロジックを含まないため |
| `MainWindow.xaml`（View） | UI 表示のため、自動テストの対象外 |
| `AssemblyInfo.cs` | 属性定義のみのため |

## 5. 集計

| 層 | テストクラス数 | テスト件数 |
|---|---|---|
| Applications | 8 | 22 |
| Domains | 5 | 21 |
| Presentations | 7 | 23 |
| 合計 | 20 | 66 |
