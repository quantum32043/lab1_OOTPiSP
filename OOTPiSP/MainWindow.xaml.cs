using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using Microsoft.Win32;
using Newtonsoft.Json;
using OOTPiSP.Factory;
using OOTPiSP.GeometryFigures.Shared;
using Formatting = Newtonsoft.Json.Formatting;

namespace OOTPiSP;

public partial class MainWindow
{
    const int DefaultAngleRotation = 2;
    const int DefaultMoveCoordinate = 2;
    
    readonly Dictionary<object, AbstractFactory> _buttonActions = new()
    {
        { "0", new CircleFactory() },
        { "1", new EllipseFactory() },
        { "2", new SquareFactory() },
        { "3", new RectangleFactory() },
        { "4", new LineFactory() },
        { "5", new TriangleFactory() }
    };
    
    AbstractFactory Factory { get; set; } = new CircleFactory();
    List<AbstractShape> AbstractShapes { get; set; } = new();
    
    void Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && _buttonActions.TryGetValue(button.Tag, out var action))
        {
             Factory = action;
        }
    }
    
    bool _isHandledButton;
    MyPoint _downMyPoint;
    MyPoint _upMyPoint;

    int _angle;
    int _arrowsX;
    int _arrowsY;
    
    MouseEventArgs? _mouseArgs;

    public MainWindow()
    {
        
        InitializeComponent();
        CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, (_, _) =>
        {
            if (MessageBox.Show("Выйти из программы?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.Yes)
                Close();
        }));
        
    }

    void Canvas_OnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            RemoveLastShape();
            return;
        }

        if (e is { ClickCount: 1, OriginalSource: Shape shape })
        {
            int tag = (int)shape.Tag;
            for (int i = tag + 1; i < Canvas.Children.Count; i++)
            {
                if (Canvas.Children[i] is Shape item)
                {
                    int tagTemp = (int)item.Tag;
                    item.Tag = --tagTemp;
                }
            }
            Canvas.Children.RemoveAt(tag);

            for (int i = tag + 1; i < AbstractShapes.Count; i++)
            {
                AbstractShapes[i].CanvasIndex--;
            }

            AbstractShapes.RemoveAt(tag);
        }
    }
    
    void Canvas_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e is { ClickCount: 2, Source: Shape frameworkElement })
        {
            var tag = (int) frameworkElement.Tag;

            var shape = AbstractShapes[tag];

            var shapeEditorWindow = new ShapeEditorWindow(shape);
            shapeEditorWindow.ShowDialog();
            
            shape.DrawAlgorithm(Canvas);

            SetHandlers(shape.CanvasIndex);
            return;
        }
        
        ResetManipulationParams();
        _downMyPoint = GetMousePosition(e);
    }

    void Canvas_OnPreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (_downMyPoint is null) //Поскольку если начать вести от другого элемента, то будет Exception
            return;
        
        _mouseArgs = e;
        if (e.LeftButton == MouseButtonState.Pressed)
        {
            if (_isHandledButton)
            {
                RemoveLastShape();
            }
            else
            {
                _isHandledButton = true;
            }

            var mousePosition = e.GetPosition(Canvas);
            var topLeftX = _downMyPoint.X + _arrowsX;
            var topLeftY = _downMyPoint.Y + _arrowsY;
            var downRightX = mousePosition.X + _arrowsX;
            var downRightY = mousePosition.Y + _arrowsY;
            DrawShape(topLeftX, topLeftY, downRightX, downRightY, Brushes.Transparent);
        }
    }

    void Canvas_OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            _upMyPoint = GetMousePosition(e);

            if (_isHandledButton)
            {
                RemoveLastShape();
                _isHandledButton = false;
                
                var topLeftX = _downMyPoint.X + _arrowsX;
                var topLeftY = _downMyPoint.Y + _arrowsY;
                var downRightX = _upMyPoint.X + _arrowsX;
                var downRightY = _upMyPoint.Y + _arrowsY;
             
                DrawShape(topLeftX, topLeftY, downRightX, downRightY, FillColorPicker.SelectedBrush);
            }
        }

        _downMyPoint = null;
        _upMyPoint = null;
        _mouseArgs = null;
    }
    
    void RemoveLastShape()
    {
        int count = Canvas.Children.Count;
        if (count > 0)
        {
            Canvas.Children.RemoveAt(count - 1);
            AbstractShapes.RemoveAt(count - 1);
        }

    }
    
    void ResetManipulationParams()
    {
        _angle = 0;
        _arrowsX = 0;
        _arrowsY = 0;
        _isHandledButton = false;
    }
    
    MyPoint GetMousePosition(MouseEventArgs e)
    {
        var mousePosition = e.GetPosition(Canvas);
        return new(mousePosition.X, mousePosition.Y);
    }

    void DrawShape(double topLeftX, double topLeftY, double downRightX, double downRightY, Brush bg)
    {
        AbstractShape shape = Factory.CreateShape(new(topLeftX, topLeftY), new(downRightX, downRightY),
            bg, PenColorPicker.SelectedBrush, _angle);
        AbstractShapes.Add(shape);
        shape.DrawAlgorithm(Canvas);
        
        Canvas.Children[^1].PreviewMouseUp += Canvas_OnPreviewMouseUp;
        Canvas.Children[^1].PreviewMouseWheel += Canvas_OnPreviewMouseWheel;

        Canvas.Children[^1].MouseEnter += Shape_MouseEnter;
        Canvas.Children[^1].MouseLeave += Shape_MouseLeave;
    }

    void SetHandlers(int canvasIndex)
    {
        Canvas.Children[canvasIndex].PreviewMouseUp += Canvas_OnPreviewMouseUp;
        Canvas.Children[canvasIndex].PreviewMouseWheel += Canvas_OnPreviewMouseWheel;
        Canvas.Children[canvasIndex].MouseEnter += Shape_MouseEnter;
        Canvas.Children[canvasIndex].MouseLeave += Shape_MouseLeave;
    }
    
    void Shape_MouseEnter(object sender, MouseEventArgs e)
    {
        if (sender is Shape s)
        {
            var effect = new DropShadowEffect
            {
                BlurRadius = 0,
                ShadowDepth = 0,
                RenderingBias = RenderingBias.Quality
            };

            var blurAnimation = new DoubleAnimation
            {
                To = 70,
                Duration = TimeSpan.FromSeconds(1),
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = new SineEase() { EasingMode = EasingMode.EaseInOut }
            };
            
            effect.BeginAnimation(DropShadowEffect.BlurRadiusProperty, blurAnimation);
            s.Effect = effect;

            s.StrokeThickness += 3;
        }
    }
    
    void Shape_MouseLeave(object sender, MouseEventArgs e)
    {
        if (sender is Shape s)
        {
            s.Effect = null;
            s.StrokeThickness -= 3;
        }
    }
    
    void Canvas_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e) => Rotate(sender, e.Delta > 0 ? DefaultAngleRotation : -DefaultAngleRotation);
    void RotateLeft_OnExecuted(object sender, ExecutedRoutedEventArgs e) => Rotate(sender, -DefaultAngleRotation);
    void RotateRight_OnExecuted(object sender, ExecutedRoutedEventArgs e) => Rotate(sender, DefaultAngleRotation);
    void RotateReset_OnExecuted(object sender, ExecutedRoutedEventArgs e) => Rotate(sender, 360 - _angle);
    void MoveUp_OnExecuted(object sender, ExecutedRoutedEventArgs e) => Move(sender, 0, -DefaultMoveCoordinate);
    void MoveDown_OnExecuted(object sender, ExecutedRoutedEventArgs e) => Move(sender, 0, DefaultMoveCoordinate);
    void MoveRight_OnExecuted(object sender, ExecutedRoutedEventArgs e) => Move(sender, DefaultMoveCoordinate, 0);
    void MoveLeft_OnExecuted(object sender, ExecutedRoutedEventArgs e) => Move(sender, -DefaultMoveCoordinate, 0);
    
    void Rotate(object sender, int angleDelta)
    {
        _angle += angleDelta;
        
        if (_angle < 0)
            _angle += 360;
        _angle %= 360;
        
        if (_mouseArgs != null)
        {
            Canvas_OnPreviewMouseMove(sender, _mouseArgs);
        }
    }

    void Move(object sender, int deltaX, int deltaY)
    {
        _arrowsX += deltaX;
        _arrowsY += deltaY;
        if (_mouseArgs != null)
        {
            Canvas_OnPreviewMouseMove(sender, _mouseArgs);
        }
    }

    void JSONSave_OnClick(object sender, RoutedEventArgs e)
    {
        SaveFileDialog saveFileDialog = new()
        {
            Filter = "JSON файлы (*.json)|*.json|Все файлы (*.*)|*.*"
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            if (!saveFileDialog.FileName.EndsWith(".json"))
            {
                saveFileDialog.FileName += ".json";
            }
            using FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create);
            
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Objects
            };
            string json = JsonConvert.SerializeObject(AbstractShapes, settings);
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            fs.Write(bytes, 0, bytes.Length);
        }
    }
    
    void BINSave_OnClick(object sender, RoutedEventArgs e)
    {
        SaveFileDialog saveFileDialog = new()
        {
            Filter = "Bin файлы (*.bin)|*.bin|Все файлы (*.*)|*.*"
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            if (!saveFileDialog.FileName.EndsWith(".bin"))
            {
                saveFileDialog.FileName += ".bin";
            }
            #pragma warning disable SYSLIB0011
            BinaryFormatter formatter = new BinaryFormatter();
            #pragma warning restore SYSLIB0011 

            using (Stream stream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                List<AbstractShapeBin> list = new();
                foreach (var item in AbstractShapes)
                {
                    list.Add(new()
                    {
                        Angle = item.Angle,
                        BackgroundColorString = item.BackgroundColor.ToString(),
                        DownRight = item.DownRight,
                        PenColorString = item.PenColor.ToString(),
                        StrokeThickness = item.StrokeThickness,
                        TagShape = item.TagShape,
                        TopLeft = item.TopLeft
                    });
                }
                formatter.Serialize(stream, list);
            }
        }
    }

    void JSONOpen_Click(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "JSON файлы (*.json)|*.json"
        };
        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
                string json = File.ReadAllText(openFileDialog.FileName);

                var settings = new JsonSerializerSettings()
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                };
                List<AbstractShape>? loadedShapes = JsonConvert.DeserializeObject<List<AbstractShape>>(json, settings);

                if (loadedShapes is { Count: not 0 })
                {
                    AbstractShapes = loadedShapes;
                    Canvas.Children.Clear();
                    foreach (var shape in AbstractShapes)
                    {
                        shape.DrawAlgorithm(Canvas);
                        
                        Canvas.Children[^1].PreviewMouseUp += Canvas_OnPreviewMouseUp;
                        Canvas.Children[^1].PreviewMouseWheel += Canvas_OnPreviewMouseWheel;

                        Canvas.Children[^1].MouseEnter += Shape_MouseEnter;
                        Canvas.Children[^1].MouseLeave += Shape_MouseLeave;
                    }
                    MessageBox.Show($"Список фигур успешно загружен!");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии файла JSON: {ex.Message}");
            }
        }
    }
    
    void BINOpen_OnClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "Bin файлы (*.bin)|*.bin"
        };
        if (openFileDialog.ShowDialog() == true)
        {
            try
            {
#pragma warning disable SYSLIB0011
                BinaryFormatter formatter = new BinaryFormatter();
#pragma warning restore SYSLIB0011 
                using FileStream fs = new FileStream(openFileDialog.FileName, FileMode.OpenOrCreate);

                if (formatter.Deserialize(fs) is List<AbstractShapeBin> { Count: not 0 } loadedShapes)
                {
                    AbstractShapes.Clear();
                    Canvas.Children.Clear();
                    foreach (var item in loadedShapes)
                    {
                        var shape = _buttonActions[item.TagShape].CreateShape(item.TopLeft, item.DownRight, item.GetBackgroundColor(),
                            item.GetPenColor(), item.Angle);
                        shape.StrokeThickness = item.StrokeThickness;
                        AbstractShapes.Add(shape);
                        shape.DrawAlgorithm(Canvas);

                        SetHandlers(shape.CanvasIndex);
                    }
                    
                    MessageBox.Show($"Список фигур успешно загружен!");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии файла BIN: {ex.Message}");
            }
        }
    }

    void ClearCanvas_OnClick(object sender, RoutedEventArgs e)
    {
        AbstractShapes.Clear();
        Canvas.Children.Clear();
    }
}