## 1. 開発環境
### .NET
- 10.0
### GUIフレームワーク
- WPF
### DIコンテナ
- Microsoft.Extensions.DependencyInjection 11.0.0
- コンポジションルートは App.xaml.cs（OnStartup で ConfigureServices により登録）
- Singleton で各 Application/Domain/Presentation サービスを登録し、コンストラクタインジェクションで解決
GainProcessor、DelayProcessor、IirProcessor は DI に登録せず
### NuGet
- commuittyToolkitMVVM
- XUnit
- Scottplot
## 2. コーディング規約
- フィールドはアンダーバーにキャメルケース
- ローカル変数はキャメルケース
- プロパティ、関数名、定数はパスカルケース
- マジックナンバーは禁止
- 変数名の省略は禁止（例：viewmodelをvm、indexをidxなど）
- 関数の引数は1個ずつ改行して宣言
## 3. レイヤ分け
### Presentation（src\SignalProcessingToolApp\Presentations）
- Views：View（xaml）、code-behind
- ViewModels：MainWindowViewModel、SubViewModels（InputSignal／CommonParameter／SignalProcessing／Plot）
- States：AppState、ProcessStateManager（状態遷移・UI有効状態の管理）
- Factories：SignalExecutionSettingsFactory（ConfigFactoryを用いてリクエストを生成）、ConfigFactory（UI値からApplicationのConfigを生成）
### Application（src\SignalProcessingToolApp\Applications）
- Sessions：SignalProcessingService（一括・フレーム単位の処理実行、波形蓄積）
- Pipelines：SignalProcessingChain、WaveformGenerator、SignalProcessor、FrameProcessor、WaveformType
- Configs：SignalProcessingConfig、GainConfig、DelayConfig、IirConfig、CommonConfig、SineWaveConfig
- DTOs：SignalExecutionSettings（Configのみを保持）、FilterCoefficientsDto（係数表示用）
- Factories：ParameterFactory（ApplicationのConfigからDomainのParameterを生成。IIRはDomainのFilterDesignerを呼び出して係数を計算）
### Domain（src\SignalProcessingToolApp\Domains）
- WaveformGeneration：SineWaveGenerator、UnitImpulseGenerator、SineWaveParameter
- SignalProcessing：GainProcessor、DelayProcessor（リングバッファによる遅延処理）、IirProcessor（x・y の過去値を保持する2次IIR処理）、FilterDesigner（サンプリング周波数・カットオフ周波数・フィルタタイプから係数を計算）
- Parameters：CommonParameter、DelayParameter、IirParameter（係数 b0・b1・b2・a1・a2のみを保持）
- Filters：FilterType（スルー、1・2サンプルディレイ、a1・a2確認用、LPF、HPF）

## 4. UI
### 方針
- 画面を2列に分割。
- 0列：パラメータ入力（4行セクション）
- 1列：ボタンとグラフ

### 左列（パラメータ入力）
#### 0行：共通パラメータ
- サンプリング周波数 [Hz]
  - コンボボックス
  - 値は 44100、48000、96000 Hz
- フレームサイズ [Sample]
  - コンボボックス
  - 値は 8、16、32、64、128、256 sample
- データサイズ [Sample]
  - コンボボックス
  - 値は 8、16、32、64、128、256、512、1024 sample

#### 1行：入力信号
- 左列は Grid で2行1列に分割
  - 0行：ラジオボタン「正弦波」（Grid 0行0列）と正弦波周波数パラメータ（Grid 0行1列）
    - 正弦波周波数 [Hz]（コンボボックス）
    - 値は 100、200、500、1000、2000、5000、10000、20000 Hz
    - インパルス選択時は非活性
  - 1行：ラジオボタン「インパルス」（Grid 1行0列）のみ配置

#### 2行：信号処理
- ゲイン
  - チェックボックス「ゲインを適用」
  - チェック時のみ以下が有効
    - ゲイン値 [dB]（コンボボックス）
    - 値は 0、-3、-6、-9、-12、-15、-18、-200 dB
  - チェック未時は入力信号がそのまま出力
- ディレイ
  - チェックボックス「ディレイを適用」
  - チェック時のみ以下が有効（テキスト入力、整数）
    - ディレイバッファサイズ [Sample]
    - ディレイサンプル数 [Sample]
  - チェック未時はディレイを適用しない
- 2次IIRフィルタ
  - チェックボックス「2次IIRを適用」
  - チェック時のみ以下が有効
    - フィルタタイプ（コンボボックス）
      - 値は b0確認用、b1確認用、b2確認用、a1確認用、a2確認用、LPF(butter)、HPF(butter)
    - カットオフ周波数 [Hz]（テキスト入力、実数）
      - 初期値は 100
      - LPF(butter)・HPF(butter) 選択時のみ有効、それ以外は非活性
      - 係数の更新は入力確定時（フォーカス喪失・Enterキー）に行う（入力途中では更新しない）
  - 係数表示（b0・b1・b2・a1・a2）はフィルタタイプ選択時、およびカットオフ周波数の入力確定時に更新
    - 係数の表示形式
      - 絶対値が 0.01 以上：小数点以下2桁固定（例：1.00、0.00、1.98）
      - 絶対値が 0.01 未満（0 を除く）：指数表記、仮数部は小数点以下2桁、指数は符号と1桁（例：5.02E-5、1.00E-4）
    - 係数式の表示領域は入力列と同程度の幅を確保し、LPF(butter)等の長い表示がはみ出ないようにする
    - 係数の表示形式
      - 絶対値が 0.01 以上：小数点以下2桁固定（例：1.00、0.00、1.98）
      - 絶対値が 0.01 未満（0 を除く）：指数表記、仮数部は小数点以下2桁、指数は符号と1桁（例：5.02E-5、1.00E-4）
    - 係数式の表示領域は入力列と同程度の幅を確保し、LPF(butter)等の長い表示がはみ出ないようにする
  - チェック未時は2次IIRを適用しない
- 処理順序：ゲイン → ディレイ → 2次IIR

### 右列（グラフとボタン）
#### 0行：操作ボタンとステータス
- ボタンの配置順：「フレーム処理」「一括処理」「リセット」
- **フレーム処理中の非活性化**：
  - 左列パラメータ入力（共通パラメータ、入力信号、信号処理）をすべて非活性にする
  - 一括処理ボタンを非活性にする
  - フレーム処理ボタンのみアクティブ（連続フレーム処理が可能）
  - リセットボタンは常にアクティブ
- ステータス表示：リセットボタンの右側に配置
  - 「今どの状態であるか」のみを表示
  - 例：「準備完了」「処理完了」「フレーム処理中」
  - 初期状態：グラフに何もプロットされていない状態を示す
  - リセットボタン押下またはバリデーションエラー時：初期状態に戻す

#### 1行：入力信号グラフ
- 正弦波またはインパルスを表示
- X軸：[0, データサイズ-1]
- Y軸：[-1.2, 1.2]

#### 2行：処理後グラフ
- ゲイン適用済みの波形、またはゲイン未適用時は入力信号のまま
- X軸：[0, データサイズ-1]
- Y軸：[-1.2, 1.2]

## 5. バリデーション
### プレゼンテーション層
- ユーザー入力については、プレゼンテーション層で数値以外の入力が入った場合は例外をスロー。
- カットオフ周波数（実数入力可）は、プレゼンテーション層で数値に変換できない場合は例外をスロー。

### ドメイン層
#### 1. 共通パラメータ
- サンプリング周波数、フレームサイズ、データサイズ、負の値なら例外をスロー。
- フレームサイズがデータサイズより大きい場合は例外をスロー。

#### 2. 正弦波のパラメータ
- 正弦波の周波数がサンプリング周波数から算出されるナイキスト周波数以上なら例外をスロー。

#### 3. 信号処理のパラメータ
- ディレイ（ディレイ有効時のみ検証）
  - バッファサイズは 0 より大きいこと。0 以下なら例外をスロー。
  - サンプル数は 0 以上であること。負の値なら例外をスロー。
  - サンプル数はバッファサイズ未満であること。以上なら例外をスロー。
- ディレイの入力値（文字列）は整数に変換できない場合、プレゼンテーション層で例外をスロー。
- 実装：リングバッファで遅延させる。フレーム間で状態（バッファ・書き込み位置）を保持し、リセット時に破棄する。
- 2次IIR（IIR有効時のみ検証）
  - カットオフ周波数は 0 以上であること。負の値なら例外をスロー（Domainで検証）。
  - カットオフ周波数はナイキスト周波数（サンプリング周波数 / 2）未満であること。以上なら例外をスロー（Domainで検証）。
  - カットオフ周波数の入力確定時（フォーカス喪失・Enterキー）にも同じ検証を行う。失敗時は係数表示をクリアし、メッセージボックスでエラー内容を表示して初期状態（グラフ・状態のリセット）に戻す。設定値は保持する。

## 6. エラー処理
### 表示方法
- **エラーメッセージ**：メッセージボックスで表示
  - ユーザーに即座に通知される
  - 確認ボタンで閉じると初期状態に戻す
- **ステータス表示**：状態表示のみに特化
  - エラーメッセージは表示しない
  - 「準備完了」「処理完了」など、現在の状態を示す

## 7. グラフの初期状態と更新ルール
- リセットボタン押下時
  - グラフをクリア（何もプロットされない状態）
  - ステータス：「準備完了」
- 初期状態でパラメータを変更した場合
  - フレーム処理または一括処理ボタンを押下しない場合、グラフは自動更新されず、プロットされない状態を保持する
  - ステータス：「準備完了」（パラメータ変更後も変わらない）
  - フレーム処理または一括処理を押下した時点で初めてグラフが更新される
- バリデーションエラー時
  - グラフをクリア（何もプロットされない状態）
  - メッセージボックスでエラー内容を表示
  - ステータス：「準備完了」
  - ユーザーがメッセージボックスを閉じると初期状態に戻す- フレーム処理時のグラフプロット
  - **リセット状態**: 蓄積配列は Null（何もプロットされない）
  - **フレーム処理 1 回目**: 蓄積配列を新規作成（サイズ = データサイズ）、インデックス 0～（フレームサイズ-1）にデータを書き込み、全蓄積データ（0～フレームサイズ分）をプロット
  - **フレーム処理 2 回目**: 蓄積配列に追記、インデックス フレームサイズ～（2×フレームサイズ-1）にデータを書き込み、全蓄積データ（0～2×フレームサイズ分）をプロット
  - **繰り返し**: データサイズに達するまで繰り返す
  - **プロット対象**: 蓄積済みのすべてのデータ（インデックス 0 から現在の処理済みサンプルまで）
  - **グラフの X 軸**: 常に [0, データサイズ-1] に固定
  - **グラフの表示内容**: 蓄積済みのサンプルのみが可視化（未処理部分は表示されない）

## 8. 状態遷移図
```mermaid
stateDiagram-v2
    [*] --> 初期状態

    %% 初期状態からの遷移
    初期状態 --> 処理完了 : 一括処理
    初期状態 --> フレーム処理中 : フレーム処理
    初期状態 --> 初期状態 : リセット
    初期状態 --> 初期状態 : バリデーションエラー

    %% 処理完了からの遷移
    処理完了 --> 処理完了 : 一括処理
    処理完了 --> フレーム処理中 : フレーム処理
    処理完了 --> 初期状態 : リセット
    処理完了 --> 初期状態 : バリデーションエラー

    %% フレーム処理中からの遷移
    フレーム処理中 --> フレーム処理中 : フレーム処理
    フレーム処理中 --> 処理完了 : フレーム処理
    フレーム処理中 --> 初期状態 : リセット
    フレーム処理中 --> 初期状態 : バリデーションエラー

```

## 9. クラス図（最新）
### クリーンアーキテクチャのレイヤ
| レイヤ          | エリア（namespace）                          | 含むクラス                                                                                                                                                                                                               | 依存先          |
| ------------ | --------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ------------ |
| Presentation | SignalProcessingToolApp.Presentations.* | MainWindow、MainWindowViewModel、SubViewModels、ProcessStateManager、SignalExecutionSettingsFactory、ConfigFactory                                                                                                        | Application  |
| Application  | SignalProcessingToolApp.Applications.*  | SignalProcessingService、SignalProcessingChain、WaveformGenerator、SignalProcessor、FrameProcessor、SignalExecutionSettings、SignalProcessingConfig、GainConfig、DelayConfig、IirConfig、CommonConfig、SineWaveConfig、ParameterFactory、FilterCoefficientsDto | Domain       |
| Domain       | SignalProcessingToolApp.Domains.*       | SineWaveGenerator、UnitImpulseGenerator、GainProcessor、DelayProcessor、IirProcessor、FilterDesigner、FilterType、SineWaveParameter、CommonParameter、DelayParameter、IirParameter | なし（他層に依存しない） |

- 依存方向は Presentation → Application → Domain の一方向とする
- App.xaml.cs（コンポジションルート）は全層を参照してDI登録のみを行う
SignalProcessor が GainProcessor、DelayProcessor、IirProcessor を内部生成する

```mermaid
classDiagram
    namespace Presentation {
        class MainWindow        
        class MainWindowViewModel
        class ProcessStateManager
        class SignalProcessingViewModel
        class InputSignalViewModel
        class CommonParameterViewModel
        class PlotViewModel
        class SignalExecutionSettingsFactory
        class ConfigFactory
    }
    namespace Application {
        class SignalProcessingService
        class SignalProcessingChain
        class WaveformGenerator
        class SignalProcessor
        class FrameProcessor
        class WaveformType
        class SignalExecutionSettings
        class SignalProcessingConfig
        class GainConfig
        class DelayConfig
        class IirConfig
        class CommonConfig
        class SineWaveConfig
        class ParameterFactory
    }
    namespace Domain {
        class SineWaveGenerator
        class UnitImpulseGenerator
        class GainProcessor
        class SineWaveParameter
        class CommonParameter
        class DelayProcessor
        class DelayParameter
        class IirProcessor
        class IirParameter
        class FilterDesigner
        class FilterType
    }

    MainWindow --> MainWindowViewModel : DataContext
    MainWindowViewModel *-- SignalProcessingViewModel
    MainWindowViewModel *-- InputSignalViewModel
    MainWindowViewModel *-- CommonParameterViewModel
    MainWindowViewModel *-- PlotViewModel
    MainWindowViewModel *-- ProcessStateManager
    MainWindowViewModel --> SignalExecutionSettingsFactory
    SignalExecutionSettingsFactory --> ConfigFactory
    ConfigFactory --> CommonParameterViewModel : reads
    ConfigFactory --> InputSignalViewModel : reads
    ConfigFactory --> SignalProcessingViewModel : reads
    MainWindowViewModel --> SignalProcessingService
    SignalExecutionSettingsFactory ..> SignalExecutionSettings : creates
    ConfigFactory ..> CommonConfig : creates
    ConfigFactory ..> SineWaveConfig : creates
    ConfigFactory ..> SignalProcessingConfig : creates
    ConfigFactory ..> GainConfig : creates
    ConfigFactory ..> DelayConfig : creates
    ConfigFactory ..> IirConfig : creates
    ConfigFactory ..> FilterType : uses
    SignalProcessingService --> SignalProcessingChain
    SignalProcessingService --> ParameterFactory
    ParameterFactory ..> CommonParameter : creates
    ParameterFactory ..> SineWaveParameter : creates
    ParameterFactory ..> DelayParameter : creates
    SignalProcessingService ..> SignalExecutionSettings
    SignalProcessingChain --> WaveformGenerator
    SignalProcessingChain ..> SignalProcessor : creates per processing unit
    SignalProcessingChain --> FrameProcessor
    WaveformGenerator ..> WaveformType
    WaveformGenerator --> SineWaveGenerator
    WaveformGenerator --> UnitImpulseGenerator
    SignalProcessor --> GainProcessor
    SignalProcessor --> DelayProcessor
    SignalProcessor ..> GainConfig
    SignalProcessor ..> DelayParameter : uses
    DelayProcessor ..> DelayParameter
    SignalExecutionSettings --> SignalProcessingConfig
    SignalProcessingConfig --> GainConfig
    SignalProcessingConfig --> DelayConfig
    SignalProcessingConfig --> IirConfig
    SignalProcessor --> IirProcessor
    ParameterFactory ..> IirParameter : creates
    ParameterFactory --> FilterDesigner
    SignalExecutionSettings --> CommonConfig
    SignalExecutionSettings --> SineWaveConfig
    SineWaveGenerator ..> SineWaveParameter
    SineWaveGenerator ..> CommonParameter
    UnitImpulseGenerator ..> CommonParameter
    SignalProcessingService ..> FilterCoefficientsDto
    ParameterFactory ..> FilterCoefficientsDto : creates
    ParameterFactory ..> FilterType
    FilterDesigner ..> FilterType
    FilterDesigner ..> IirParameter : creates
    IirConfig ..> FilterType
    SignalProcessor ..> IirParameter : uses
    IirProcessor ..> IirParameter
```