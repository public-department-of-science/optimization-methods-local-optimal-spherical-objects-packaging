using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Simplex
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void btn_generate_Click(object sender, EventArgs e)
        {
            
            try
            {
              
                #region Добавление строк и столбцов
                // столбец B
                dataGrv_inp_B.Columns.Add("","");

                for (int i = 0; i < Convert.ToInt32(txtb_n.Text); i++)
                {
                    dataGrv_inp_B.Rows.Add();
                }

                ///// целевая функция
                for (int i = 0; i < Convert.ToInt32(txtb_m.Text); i++)
                {
                    dataGrv_inp_function.Columns.Add("", "");
                }
                dataGrv_inp_function.Rows.Add();

                ///// матрица
                for (int i = 0; i < Convert.ToInt32(txtb_m.Text); i++)
                {
                    dataGrv_inp_matrix.Columns.Add("", "");
                }
                for (int j = 0; j < Convert.ToInt32(txtb_n.Text); j++)
                {
                    dataGrv_inp_matrix.Rows.Add();
                }
                #endregion
                #region Показ элементов формы
                txtb_m.ReadOnly = true;
                txtb_n.ReadOnly = true;
                dataGrv_inp_B.ReadOnly = false;
                lbl_inp_matrix.Visible = true;
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                label6.Visible = true;
                label7.Visible = true;
                #endregion
                #region Показ элементов формы
                dataGrv_inp_matrix.Visible = true;
                dataGrv_inp_B.Visible = true;
                dataGrv_inp_function.Visible = true;
                btn_generate.Visible = false;
                btn_calculate.Visible = true;
                btn_repeat.Visible = !btn_repeat.Visible;
                #endregion
            }
            catch
            {
                txtb_n.Text = "";
                txtb_m.Text = "";
                MessageBox.Show("Ошибка ввода!! Укажите корректые данные на вход!");
            }
        }

        private void btn_repeat_Click(object sender, EventArgs e)
        {
            #region Скрытие элементов формы
            dataGrv_inp_matrix.ReadOnly = false;
            dataGrv_simplex_table.Visible = false;
            dataGrv_inp_B.ReadOnly = false;
            txtb_m.ReadOnly = false;
            txtb_n.ReadOnly = false;
            btn_calculate.Visible = false;
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false;
            lbl_inp_matrix.Visible = false;
           
            dataGrv_simplex_table.Visible = false;
            dataGrv_inp_matrix.Visible = false;
            dataGrv_inp_function.Visible = false;
            dataGrv_inp_B.Visible = false;
            #endregion
            #region Очищение массива
            dataGrv_inp_matrix.Rows.Clear();
            for (int i = 0; i < dataGrv_inp_matrix.ColumnCount; i++) // цикл удаления всех столбцов { this.dataGridView1.Columns.RemoveAt(0); }
                dataGrv_inp_matrix.Columns.RemoveAt(0);

            dataGrv_inp_function.Rows.Clear();
            for (int i = 0; i < dataGrv_inp_function.ColumnCount; i++) // цикл удаления всех столбцов { this.dataGridView1.Columns.RemoveAt(0); }
                dataGrv_inp_function.Columns.RemoveAt(0);

            //// delete memory
            dataGrv_simplex_table.Rows.Clear();
            for (int i = 0; i < dataGrv_simplex_table.ColumnCount; i++) // цикл удаления всех столбцов { this.dataGridView1.Columns.RemoveAt(0); }
                dataGrv_simplex_table.Columns.RemoveAt(0);
            ////

            dataGrv_inp_B.Columns.Clear();


            btn_repeat.Visible = !btn_repeat.Visible;
            btn_generate.Visible = !btn_generate.Visible;
            #endregion
            Form1 form = new Form1();
            this.Hide();
            form.ShowDialog();
            
           
        }

        private void btn_calculate_Click(object sender, EventArgs e)
        {

            #region Отображение элементов формы
            dataGrv_inp_matrix.ReadOnly = true;
            dataGrv_inp_B.ReadOnly = true;
            label4.Visible = true;
            label5.Visible = true;
            dataGrv_simplex_table.Visible = true;
            btn_calculate.Visible = false;
            #endregion

            int n = dataGrv_inp_matrix.ColumnCount;
            int m = dataGrv_inp_matrix.RowCount;
            double min_elem= Convert.ToDouble(dataGrv_inp_matrix[0, 0].Value); // поиск минимального в матрице
            double[,] a = new double[m, n];
            #region Считка матрицы с формы
            try
            {
                for (int i = 0; i < dataGrv_inp_matrix.Rows.Count; i++)
                    for (int j = 0; j < dataGrv_inp_matrix.Columns.Count; j++)
                    {
                        if (string.IsNullOrEmpty((dataGrv_inp_matrix[i, j]).ToString()) || (dataGrv_inp_matrix[i, j]).ToString().Trim() == null)
                        {
                            throw new Exception("Обнаружены пропуски в исходной матрице");
                        }
                        a[i, j] = Convert.ToDouble(dataGrv_inp_matrix[j, i].Value);
                        if (a[i, j] < min_elem)
                            min_elem = a[i, j];

                    }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Это НЕ число!!!\n" + "ОШИБКА: " + ex.Message + "\n\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show("ОШИБКА: " + ex.Message + "\n\n");
            }

            #endregion 
            ////
            #region Создание специальной матрицы для входа в симпл. метод

            double[] output_Y = new double[m]; // вектор-результат вероятностей для второго игрока
            double[] output_X = new double[n]; // вектор-результат вероятностей для первого игрока
            double V; // цена игры 
         

            double[,] Vector_result; //последняя итерация симплекс-метода
            double[,] value = new double[n + 1, m + 1]; // Соединенная матрица введенных данных в единую

            int y = 0;// счетчики итераций
            int k = 0;

            for (int i = 1; i <= n ; i++)
            {
                for (int j = 1; j <= m ; j++)
                {
                    if(min_elem>0)
                        value[k, j] = a[i - 1, j - 1];
                    else
                        value[k, j] = a[i - 1, j - 1]+Math.Abs(min_elem)+1;
                }
                k = k + 1;
            }

            for (int i = 0; i < dataGrv_inp_B.Rows.Count; i++)
            {
                for (int j = 0; j < 1; j++)
                {
                    value[i, j] = Convert.ToDouble(dataGrv_inp_B[j, i].Value);
                };
            }

            k = 0;
            y = 0;

            for (int i = n; i < n + 1; i++)
                for (int j = 1; j < m + 1; j++)
                {
                    value[i, j] = Convert.ToDouble(dataGrv_inp_function[y, k].Value);
                    y++;
                }

            #endregion

            Simplex S = new Simplex(value);
            Vector_result = S.Calculate(output_Y);
            V = 1 / Vector_result[Vector_result.GetLength(0) - 1, 0];


            #region Формирование ответа
            for (int i = 0; i < output_Y.Length; i++)
                output_Y[i] *= V;
            for (int i = Vector_result.GetLength(0)-1; i < Vector_result.GetLength(0); i++)
            {
                k = n;
                for (int j = 0; j < output_X.Length; j++)
                {
                    output_X[j] = V*Vector_result[i, k+1];
                    ++k;
                }
            }
           

          for (int i = 0; i < Vector_result.GetLength(1); i++)
            {
                dataGrv_simplex_table.Columns.Add("", "");
            }
            for (int i = 0; i < Vector_result.GetLength(0); i++)
                dataGrv_simplex_table.Rows.Add();

            for (int j = 0; j < dataGrv_simplex_table.RowCount; j++)
            {
                k = 0;
                for (int i = 0; i < dataGrv_simplex_table.ColumnCount; i++)
                {
                   dataGrv_simplex_table[i, j].Value = (Math.Round(Vector_result[j, k], 2));
                    ++k;
                }
                ++y;
            }

            string str ="Ответ= Y{";
            for (int i = 0; i < output_Y.Length; i++)
            {
                str+= string.Format(Math.Round(output_Y[i], 2).ToString()+",");
            }
            str += "}\nОтвет= X{";
            for (int i = 0; i < output_X.Length; i++)
            {
                str += string.Format(Math.Round(output_X[i], 2).ToString() + ",");
            }
            if (min_elem <= 0)
                V =V-Math.Abs( min_elem)-1;

            str += "}\nV="+Math.Round(V,3).ToString();
            MessageBox.Show(str);
       
            #endregion

            dataGrv_simplex_table.Visible = true;

        }
        private class Simplex
        {
            //source - симплекс таблица без базисных переменных
            double[,] result; //симплекс таблица

            int m, n; // размерности

            List<int> BAsiс_column; //список базисных переменных

            public Simplex(double[,] data)
            {
                m = data.GetLength(0); // 
                n = data.GetLength(1); //
                result = new double[m, n + m - 1];
                BAsiс_column = new List<int>();

                for (int i = 0; i < m; i++)
                {

                    for (int j = 0; j < result.GetLength(1); j++)
                    {
                        if (j < n)
                            result[i, j] = data[i, j];
                        else
                            result[i, j] = 0;
                    }
                    //расчет количества базисных элементов
                    if ((n + i) < result.GetLength(1))
                    {
                        result[i, n + i] = 1;
                        BAsiс_column.Add(n + i);
                    }
                }

                n = result.GetLength(1);
            }

            //result - в этот массив будут записаны полученные значения X
            public double[,] Calculate(double[] result)
            {
                int GeneralCol, GeneralRow; //ведущие столбец и строка

                while (!OptimalSourseFinded())
                {
                    GeneralCol = GeneralnCol();
                    GeneralRow = this.GeneralRow(GeneralCol);
                    BAsiс_column[GeneralRow] = GeneralCol;

                    double[,] new_table = new double[m, n];

                    for (int j = 0; j < n; j++)
                        new_table[GeneralRow, j] = this.result[GeneralRow, j] / this.result[GeneralRow, GeneralCol];

                    for (int i = 0; i < m; i++)
                    {
                        if (i == GeneralRow)
                            continue;

                        for (int j = 0; j < n; j++)
                            new_table[i, j] = this.result[i, j] - this.result[i, GeneralCol] * new_table[GeneralRow, j];
                    }
                    this.result = new_table;
                }

                //найденные значения Y
                for (int i = 0; i < result.Length; i++)
                {
                    int k = BAsiс_column.IndexOf(i + 1);
                    if (k != -1)
                        result[i] = this.result[k, 0];
                    else
                        result[i] = 0;
                }

                return this.result;
            }

            private bool OptimalSourseFinded()
            {
                bool logical = true;

                for (int j = 1; j < n; j++)
                {
                    if (result[m - 1, j] < 0)
                    {
                        logical = false;
                        break;
                    }
                }

                return logical;
            }

            private int GeneralnCol()
            {
                int col = 1;

                for (int j = 2; j < n; j++)
                    if (result[m - 1, j] < result[m - 1, col])
                        col = j;

                return col;
            }

            private int GeneralRow(int mainCol)
            {
                int row = 0;

                for (int i = 0; i < m - 1; i++)
                    if (result[i, mainCol] > 0)
                    {
                        row = i;
                        break;
                    }

                for (int i = row + 1; i < m - 1; i++)
                    if ((result[i, mainCol] > 0) && ((result[i, 0] / result[i, mainCol]) < (result[row, 0] / result[row, mainCol])))
                        row = i;

                return row;
            }


        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
                Application.Exit();
        }

    }
}