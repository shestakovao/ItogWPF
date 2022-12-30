using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ItogWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool inputOperationCondition = false; //Условие ввода операции 
        string memoryString = "";//строка памяти

        private void FindErrorInInput()
        {
            if (InputText.Text == "Ошибка отрицательно число под корнем" || InputText.Text == "Ошибка деления на ноль")
            {
                InputText.Text = "0";
            }
        }

        private string ElementaryOperation(string addString, string operationString)//элементарная операция в строке первой по порядку операции
        {
            string outString = addString;
            string number1String = "";//первое число строкой
            double number1Double = 0;
            string number2String = "";//второе число строкой
            double number2Double = 0;

            if (!((operationString == " √ ") || (operationString == " ∛ ")))
            {
                number1String = CopyLeftNumberElementaryString(outString, outString.IndexOf(operationString), operationString);//первое число строкой
                number1Double = Convert.ToDouble(number1String);

            }
            if (!((operationString == " ² ") || (operationString == " ³ ")))
            {
                number2String = CopyRightNumberElementaryString(outString, outString.IndexOf(operationString), operationString);//второе число строкой
                if (number2String == "")
                {
                    if (operationString == " * " || operationString == " / ") number2Double = number1Double;
                    if (operationString == " + " || operationString == " - ") number2Double = number1Double;
                    if (operationString == " √ " || operationString == " ∛ ") number2Double = 0;
                }
                else number2Double = Convert.ToDouble(number2String);
            }
            string resultOperation = "";

            if (operationString == " √ ")
            {
                if (number2Double < 0) resultOperation = "Ошибка отрицательно число под корнем";
                else resultOperation = Convert.ToString(Math.Sqrt(number2Double));
            }
            if (operationString == " ∛ ") resultOperation = Convert.ToString(Math.Cbrt(number2Double));
            if (operationString == " ² ") resultOperation = Convert.ToString(Math.Pow(number1Double, 2));
            if (operationString == " ³ ") resultOperation = Convert.ToString(Math.Pow(number1Double, 3));
            if (operationString == " * ") resultOperation = Convert.ToString(number1Double * number2Double);
            if (operationString == " / ")
            {
                if (number2Double == 0.0) resultOperation = "Ошибка деления на ноль";
                else resultOperation = Convert.ToString(number1Double / number2Double);
            }
            if (operationString == " + ") resultOperation = Convert.ToString(number1Double + number2Double);
            if (operationString == " - ") resultOperation = Convert.ToString(number1Double - number2Double);
            //заменяем найденую операцию на результат
            if (resultOperation == "Ошибка отрицательно число под корнем" || resultOperation == "Ошибка деления на ноль") outString = resultOperation; 
            else outString = addString.Substring(0, addString.IndexOf(operationString) - number1String.Length) + resultOperation + addString.Substring(addString.IndexOf(operationString) + operationString.Length + number2String.Length);
            return outString;
        }

        private string CopyLeftNumberElementaryString(string addString, int tempIndex, string operationString)//копирование числа из левой части строки
        {
            string tempString = addString.Substring(0, tempIndex);
            if (tempString.LastIndexOf(" ") > -1)//есть операции значит берём и копируем последнее число строки
            {
                return tempString.Substring(tempString.LastIndexOf(" ") + 1);
            }
            else return tempString;//нет операций значит уже готовое число
        }
        private string CopyRightNumberElementaryString(string addString, int tempIndex, string operationString)//копирование числа из правой части строки
        {
            string tempString = addString.Substring(tempIndex + operationString.Length);
            if (tempString.IndexOf(" ") > -1)//есть операции значит берём и копируем первое число строки
            {
                return tempString.Substring(0, tempString.IndexOf(" "));
            }
            else return tempString;//нет операций значит уже готовое число
        }

        private string CalculationElementaryString(string addString)//расчёт элементарной строки
        {
            string tempCalculationElementaryString = addString;
            string firstOperation = "";
            string secondOperation = "";
            for (int i = 0; i < 4; i++)// в цикле i = 0 операции ² и ³ i = 1 операции √ и ∛ i = 2 операции * и / i = 3 операции + и - 
            {
                if (i == 0)
                {
                    firstOperation = " ² ";
                    secondOperation = " ³ ";
                }
                if (i == 1)
                {
                    firstOperation = " √ ";
                    secondOperation = " ∛ ";
                }
                if (i == 2)
                {
                    firstOperation = " * ";
                    secondOperation = " / ";
                }
                if (i == 3)
                {
                    firstOperation = " + ";
                    secondOperation = " - ";
                }
                while ((tempCalculationElementaryString.IndexOf(firstOperation) > -1) || (tempCalculationElementaryString.IndexOf(secondOperation) > -1))
                //пока есть операции данного порядка выполняем цикл вычисляя элементарные операции
                {
                    if ((tempCalculationElementaryString.IndexOf(firstOperation) > -1) && (tempCalculationElementaryString.IndexOf(secondOperation) > -1))
                    //если есть операции умножения и деления
                    {
                        if (tempCalculationElementaryString.IndexOf(firstOperation) > tempCalculationElementaryString.IndexOf(secondOperation))
                        {
                            tempCalculationElementaryString = ElementaryOperation(tempCalculationElementaryString, secondOperation);
                        }
                        else
                        {
                            tempCalculationElementaryString = ElementaryOperation(tempCalculationElementaryString, firstOperation);
                        }
                    }
                    else if ((tempCalculationElementaryString.IndexOf(firstOperation) > -1))
                    {
                        tempCalculationElementaryString = ElementaryOperation(tempCalculationElementaryString, firstOperation);
                    }
                    else
                    {
                        tempCalculationElementaryString = ElementaryOperation(tempCalculationElementaryString, secondOperation);
                    }
                }
            }

            return tempCalculationElementaryString;

        }


        private void AddStringToText(string addString)//добавление числовой строки
        {
            FindErrorInInput();
            if (InputText.Text != "0" && !(inputOperationCondition))
            {
                InputText.Text += addString;
                inputOperationCondition = false;
            }
            else
            {
                InputText.Text = addString;
                inputOperationCondition = false;
            }
        }


        private void AddOperationToText(string addOperation)//добавление операции в строку
        {
            FindErrorInInput();
            if (OutText.Text.IndexOf(" = ") > -1) OutText.Text = "";
            string tempString = OutText.Text;
            string tempSubString = "";
            if (tempString == "")//если выходная строка пустая
            {
                if ((addOperation == " √ ") || (addOperation == " ∛ "))
                {
                    if (InputText.Text == "0")
                    {
                        tempString = addOperation;
                        inputOperationCondition = true;
                    }
                    else
                    {
                        tempString = addOperation + InputText.Text;
                        inputOperationCondition = false;
                        InputText.Text = "0";
                    }
                }
                else
                {
                    tempString = InputText.Text + addOperation;
                    InputText.Text = "0";
                    inputOperationCondition = true;
                }

            }
            else
            if (inputOperationCondition)//замена операции или добавление √ или ∛
            {



                if (tempString.Length >= 3)
                {
                    tempSubString = tempString.Substring(tempString.Length - 3, 3);
                    if (((addOperation == " √ ") || (addOperation == " ∛ "))
                        && ((tempSubString != " √ ") && (tempSubString != " ∛ ") && (tempSubString != " ² ") && (tempSubString != " ³ ")))
                    {
                        tempString += addOperation; //вставка операции корней
                        InputText.Text = "0";
                    }
                    else if (((addOperation == " √ ") || (addOperation == " ∛ "))
                        && ((tempSubString == " √ ") || (tempSubString == " ∛ ")))
                    {
                        tempString = tempString.Substring(0, tempString.Length - 3);
                        tempString += addOperation;//Замена операции корней
                    }
                    else if (((addOperation == " ² ") || (addOperation == " ³ "))
                        && ((tempSubString == " ² ") || (tempSubString == " ³ ")))
                    {
                        tempString = tempString.Substring(0, tempString.Length - 3);
                        tempString += addOperation;//Замена операции степеней
                    }
                    else if (((addOperation == " + ") || (addOperation == " - ") || (addOperation == " * ") || (addOperation == " / "))
                        && ((tempSubString == " ² ") || (tempSubString == " ³ ")))
                    {
                        tempString = OutText.Text + addOperation; //добавление операций
                        inputOperationCondition = true;
                    }
                    else if (((addOperation == " + ") || (addOperation == " - ") || (addOperation == " * ") || (addOperation == " / "))
                        && !((tempSubString == " √ ") || (tempSubString == " ∛ ")))
                    {
                        if (tempSubString != addOperation)//Замена типа операции
                        {
                            tempString = tempString.Substring(0, tempString.Length - 3);
                            tempString += addOperation;
                        }
                    }
                }
            }
            else//добавление действия кроме добавления √ или ∛
            {
                if (!((addOperation == " √ ") || (addOperation == " ∛ ")))
                {

                    if (OutText.Text[OutText.Text.Length - 1] == ' ' && OutText.Text[OutText.Text.Length - 2] != '²' && OutText.Text[OutText.Text.Length - 2] != '³')
                    {
                        tempString = OutText.Text + InputText.Text + addOperation;
                        InputText.Text = "0";
                        inputOperationCondition = true;
                    }
                    else
                    {
                        tempString = OutText.Text + addOperation;
                        inputOperationCondition = true;
                        if (!((addOperation == " ² ") || (addOperation == " ³ ")))
                            if (InputText.Text != "0")
                            {
                                tempString += InputText.Text;
                                InputText.Text = "0";
                                inputOperationCondition = false;
                            }
                    }


                }
            }
            OutText.Text = tempString;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InputText.Text = "0";
            OutText.Text = "";
            memoryString = ""; //строка памяти 
            inputOperationCondition = false;//Условие ввода операции 
            MemRead.IsEnabled = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AddStringToText("0");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            AddStringToText("1");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            AddStringToText("2");
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            AddStringToText("3");
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            AddStringToText("4");
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            AddStringToText("5");
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            AddStringToText("6");
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            AddStringToText("7");
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            AddStringToText("8");
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            AddStringToText("9");
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            FindErrorInInput();
            bool findDecimialSeparator = true;
            foreach (char a in InputText.Text) //поиск есть ли уже разделитель разряда в исходной строке
            {
                if (a == ',')
                {
                    findDecimialSeparator = false;
                }
            }
            if (findDecimialSeparator) 
            {
                InputText.Text += ",";//если разделитель на найден вставляем его
                inputOperationCondition = false;
            }
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            FindErrorInInput();
            string tempString = InputText.Text;
            if (tempString != "0")//если строка равна нулю то не ставим знак
            {
                if (tempString[0] == '-') //если у числа уже есть знак минус удаляем его
                {
                    tempString = tempString.Substring(1);
                }
                else //если у числа нет знака минус добавляем его
                {
                    tempString = "-" + tempString;
                }
                inputOperationCondition = false;
            }          
            InputText.Text = tempString;
        }

        private void Button_Click_13(object sender, RoutedEventArgs e)
        {
            AddOperationToText(" + ");
        }

        private void Button_Click_15(object sender, RoutedEventArgs e)
        {
            AddOperationToText(" - ");
        }

        private void Button_Click_17(object sender, RoutedEventArgs e)
        {
            AddOperationToText(" * ");
        }

        private void Button_Click_18(object sender, RoutedEventArgs e)
        {
            AddOperationToText(" / ");
        }

        private void Button_Click_24(object sender, RoutedEventArgs e)
        {
            AddOperationToText(" ² ");
        }

        private void Button_Click_25(object sender, RoutedEventArgs e)
        {
            AddOperationToText(" ³ ");
        }

        private void Button_Click_26(object sender, RoutedEventArgs e)
        {
            AddOperationToText(" ∛ ");
        }

        private void Button_Click_27(object sender, RoutedEventArgs e)
        {
            AddOperationToText(" √ ");
        }

        private void Button_Click_14(object sender, RoutedEventArgs e)
        {
            if (!(OutText.Text.IndexOf(" = ") > -1))
            {
                if (InputText.Text != "0")
                {
                    if (OutText.Text[OutText.Text.Length - 1] == ' ' && OutText.Text[OutText.Text.Length - 2] != '²' && OutText.Text[OutText.Text.Length - 2] != '³')
                    {
                        OutText.Text += InputText.Text;
                    }
                }
                InputText.Text = CalculationElementaryString(OutText.Text);
                OutText.Text += " = ";
            }
        }

        private void Button_Click_16(object sender, RoutedEventArgs e)
        {
            string tempString = InputText.Text;
            if (!inputOperationCondition) //если не введена операция
            {
                if (tempString.Length <= 1) //если количество символов равно 1 то ставим 0
                {
                    tempString = "0";
                }
                else if ((tempString.Length == 2) && (tempString[0] == '-')) //если количество символов равно 2 а первый символ минус то ставим 0
                {
                    tempString = "0";
                }
                else
                {
                    tempString = tempString.Substring(0, tempString.Length - 1);
                }
                if (tempString == "-0") tempString = "0";
                InputText.Text = tempString;
            }
        }

        private void Button_Click_19(object sender, RoutedEventArgs e)
        {
            InputText.Text = "0";
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.NumPad0 || (e.Key == Key.D0)) //Нажата кнопка на клавиатуре 0
            {
                Button_Click_1(null, null);
            }
            if (e.Key == Key.NumPad1 || e.Key == Key.D1) //Нажата кнопка на клавиатуре 1
            {
                Button_Click_2(null, null);
            }
            if (e.Key == Key.NumPad2 || e.Key == Key.D2) //Нажата кнопка на клавиатуре 2
            {
                Button_Click_3(null, null);
            }
            if (e.Key == Key.NumPad3 || e.Key == Key.D3) //Нажата кнопка на клавиатуре 3
            {
                Button_Click_4(null, null);
            }
            if (e.Key == Key.NumPad4 || e.Key == Key.D4) //Нажата кнопка на клавиатуре 4
            {
                Button_Click_5(null, null);
            }
            if (e.Key == Key.NumPad5 || e.Key == Key.D5) //Нажата кнопка на клавиатуре 5
            {
                Button_Click_6(null, null);
            }
            if (e.Key == Key.NumPad6 || e.Key == Key.D6) //Нажата кнопка на клавиатуре 6
            {
                Button_Click_7(null, null);
            }
            if (e.Key == Key.NumPad7 || e.Key == Key.D7) //Нажата кнопка на клавиатуре 7
            {
                Button_Click_8(null, null);
            }
            if (e.Key == Key.NumPad8 || e.Key == Key.D8) //Нажата кнопка на клавиатуре 8
            {
                Button_Click_9(null, null);
            }
            if (e.Key == Key.NumPad9 || (e.Key == Key.D9 && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)) //Нажата кнопка на клавиатуре 9
            {
                Button_Click_10(null, null);
            }
            if (e.Key == Key.Decimal) //Нажата кнопка на клавиатуре разделитель разрядов
            {
                Button_Click_11(null, null);
            }
            if (e.Key == Key.OemPlus || e.Key == Key.Add) //Нажата кнопка на клавиатуре +
            {
                Button_Click_13(null, null);
            }
            if (e.Key == Key.OemMinus || e.Key == Key.Subtract) //Нажата кнопка на клавиатуре -
            {
                Button_Click_15(null, null);
            }
            if (e.Key == Key.Multiply) //Нажата кнопка на клавиатуре *
            {
                Button_Click_17(null, null);
            }
            if (e.Key == Key.Divide) //Нажата кнопка на клавиатуре /
            {
                Button_Click_18(null, null);
            }
            if (e.Key == Key.Enter) //Нажата кнопка на клавиатуре Enter
            {
                Button_Click_14(null, null);
            }

            if (e.Key == Key.Back) //Нажата кнопка на клавиатуре BackSpace
            {
                Button_Click_16(null, null);
            }

            if (e.Key == Key.Delete) //Нажата кнопка на клавиатуре Del
            {
                Button_Click_19(null, null);
            }


            if (e.Key == Key.D6 && e.KeyboardDevice.Modifiers == ModifierKeys.Shift) //Нажата кнопка на клавиатуре ^
            {
                Button_Click_27(null, null);
            }

        }

        private void Button_Click_20(object sender, RoutedEventArgs e)
        {
            InputText.Text = Math.E.ToString();
        }


        private void Button_Click_23(object sender, RoutedEventArgs e)
        {
            InputText.Text = Math.PI.ToString();
        }

        private void Button_Click_21(object sender, RoutedEventArgs e)
        {
            FindErrorInInput();
            memoryString = InputText.Text;//поле ввода заносим в памяти калькулятора
            MemRead.IsEnabled = true;
        }

        private void Button_Click_22(object sender, RoutedEventArgs e)
        {
            if (memoryString != "")//если строка памяти калькулятора не пустая то заносим её в поле ввода
            {
                InputText.Text = memoryString;
                inputOperationCondition = false;
            }
        }
    }
}
