namespace Simplex
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_generate = new System.Windows.Forms.Button();
            this.lbl_n = new System.Windows.Forms.Label();
            this.lbl_m = new System.Windows.Forms.Label();
            this.dataGrv_inp_matrix = new System.Windows.Forms.DataGridView();
            this.txtb_n = new System.Windows.Forms.TextBox();
            this.txtb_m = new System.Windows.Forms.TextBox();
            this.btn_repeat = new System.Windows.Forms.Button();
            this.lbl_inp_matrix = new System.Windows.Forms.Label();
            this.dataGrv_inp_function = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_calculate = new System.Windows.Forms.Button();
            this.dataGrv_simplex_table = new System.Windows.Forms.DataGridView();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dataGrv_inp_B = new System.Windows.Forms.DataGridView();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrv_inp_matrix)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrv_inp_function)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrv_simplex_table)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrv_inp_B)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_generate
            // 
            this.btn_generate.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_generate.Location = new System.Drawing.Point(221, 19);
            this.btn_generate.Name = "btn_generate";
            this.btn_generate.Size = new System.Drawing.Size(171, 60);
            this.btn_generate.TabIndex = 0;
            this.btn_generate.Text = "Сгенерировать матрицу";
            this.btn_generate.UseVisualStyleBackColor = true;
            this.btn_generate.Click += new System.EventHandler(this.btn_generate_Click);
            // 
            // lbl_n
            // 
            this.lbl_n.AutoSize = true;
            this.lbl_n.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_n.Location = new System.Drawing.Point(13, 19);
            this.lbl_n.Name = "lbl_n";
            this.lbl_n.Size = new System.Drawing.Size(64, 24);
            this.lbl_n.TabIndex = 1;
            this.lbl_n.Text = "Строк";
            // 
            // lbl_m
            // 
            this.lbl_m.AutoSize = true;
            this.lbl_m.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_m.Location = new System.Drawing.Point(13, 55);
            this.lbl_m.Name = "lbl_m";
            this.lbl_m.Size = new System.Drawing.Size(98, 24);
            this.lbl_m.TabIndex = 2;
            this.lbl_m.Text = "Столбцов";
            // 
            // dataGrv_inp_matrix
            // 
            this.dataGrv_inp_matrix.AllowUserToAddRows = false;
            this.dataGrv_inp_matrix.AllowUserToDeleteRows = false;
            this.dataGrv_inp_matrix.AllowUserToResizeColumns = false;
            this.dataGrv_inp_matrix.AllowUserToResizeRows = false;
            this.dataGrv_inp_matrix.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGrv_inp_matrix.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrv_inp_matrix.Location = new System.Drawing.Point(20, 225);
            this.dataGrv_inp_matrix.Name = "dataGrv_inp_matrix";
            this.dataGrv_inp_matrix.Size = new System.Drawing.Size(257, 144);
            this.dataGrv_inp_matrix.TabIndex = 3;
            this.dataGrv_inp_matrix.Visible = false;
            // 
            // txtb_n
            // 
            this.txtb_n.Location = new System.Drawing.Point(125, 24);
            this.txtb_n.Name = "txtb_n";
            this.txtb_n.Size = new System.Drawing.Size(46, 20);
            this.txtb_n.TabIndex = 4;
            // 
            // txtb_m
            // 
            this.txtb_m.Location = new System.Drawing.Point(125, 59);
            this.txtb_m.Name = "txtb_m";
            this.txtb_m.Size = new System.Drawing.Size(46, 20);
            this.txtb_m.TabIndex = 5;
            // 
            // btn_repeat
            // 
            this.btn_repeat.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_repeat.Location = new System.Drawing.Point(417, 19);
            this.btn_repeat.Name = "btn_repeat";
            this.btn_repeat.Size = new System.Drawing.Size(171, 60);
            this.btn_repeat.TabIndex = 6;
            this.btn_repeat.Text = "Ввести новую матрицу";
            this.btn_repeat.UseVisualStyleBackColor = true;
            this.btn_repeat.Visible = false;
            this.btn_repeat.Click += new System.EventHandler(this.btn_repeat_Click);
            // 
            // lbl_inp_matrix
            // 
            this.lbl_inp_matrix.AutoSize = true;
            this.lbl_inp_matrix.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_inp_matrix.Location = new System.Drawing.Point(32, 197);
            this.lbl_inp_matrix.Name = "lbl_inp_matrix";
            this.lbl_inp_matrix.Size = new System.Drawing.Size(209, 25);
            this.lbl_inp_matrix.TabIndex = 7;
            this.lbl_inp_matrix.Text = "Заполните матрицу";
            this.lbl_inp_matrix.Visible = false;
            // 
            // dataGrv_inp_function
            // 
            this.dataGrv_inp_function.AllowUserToAddRows = false;
            this.dataGrv_inp_function.AllowUserToDeleteRows = false;
            this.dataGrv_inp_function.AllowUserToResizeColumns = false;
            this.dataGrv_inp_function.AllowUserToResizeRows = false;
            this.dataGrv_inp_function.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGrv_inp_function.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrv_inp_function.Location = new System.Drawing.Point(17, 143);
            this.dataGrv_inp_function.Name = "dataGrv_inp_function";
            this.dataGrv_inp_function.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGrv_inp_function.Size = new System.Drawing.Size(257, 41);
            this.dataGrv_inp_function.TabIndex = 8;
            this.dataGrv_inp_function.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(17, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(265, 36);
            this.label1.TabIndex = 9;
            this.label1.Text = "Укажите коэф. целевой функции\r\n(Предвварительно умножив их на -1)";
            this.label1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(280, 153);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 24);
            this.label2.TabIndex = 10;
            this.label2.Text = "--->";
            this.label2.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(325, 153);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 24);
            this.label3.TabIndex = 11;
            this.label3.Text = "min";
            this.label3.Visible = false;
            // 
            // btn_calculate
            // 
            this.btn_calculate.BackColor = System.Drawing.SystemColors.Info;
            this.btn_calculate.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_calculate.ForeColor = System.Drawing.SystemColors.Highlight;
            this.btn_calculate.Location = new System.Drawing.Point(20, 377);
            this.btn_calculate.Name = "btn_calculate";
            this.btn_calculate.Size = new System.Drawing.Size(221, 57);
            this.btn_calculate.TabIndex = 12;
            this.btn_calculate.Text = "Рассчитать";
            this.btn_calculate.UseVisualStyleBackColor = false;
            this.btn_calculate.Visible = false;
            this.btn_calculate.Click += new System.EventHandler(this.btn_calculate_Click);
            // 
            // dataGrv_simplex_table
            // 
            this.dataGrv_simplex_table.AllowUserToAddRows = false;
            this.dataGrv_simplex_table.AllowUserToDeleteRows = false;
            this.dataGrv_simplex_table.AllowUserToResizeColumns = false;
            this.dataGrv_simplex_table.AllowUserToResizeRows = false;
            this.dataGrv_simplex_table.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGrv_simplex_table.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrv_simplex_table.Location = new System.Drawing.Point(480, 225);
            this.dataGrv_simplex_table.Name = "dataGrv_simplex_table";
            this.dataGrv_simplex_table.ReadOnly = true;
            this.dataGrv_simplex_table.Size = new System.Drawing.Size(257, 144);
            this.dataGrv_simplex_table.TabIndex = 13;
            this.dataGrv_simplex_table.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(435, 278);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 24);
            this.label4.TabIndex = 14;
            this.label4.Text = "--->";
            this.label4.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(467, 197);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(284, 20);
            this.label5.TabIndex = 15;
            this.label5.Text = "Результирующая симплекс-таблица";
            this.label5.Visible = false;
            // 
            // dataGrv_inp_B
            // 
            this.dataGrv_inp_B.AllowUserToAddRows = false;
            this.dataGrv_inp_B.AllowUserToDeleteRows = false;
            this.dataGrv_inp_B.AllowUserToResizeColumns = false;
            this.dataGrv_inp_B.AllowUserToResizeRows = false;
            this.dataGrv_inp_B.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGrv_inp_B.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrv_inp_B.Location = new System.Drawing.Point(329, 225);
            this.dataGrv_inp_B.Name = "dataGrv_inp_B";
            this.dataGrv_inp_B.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.dataGrv_inp_B.Size = new System.Drawing.Size(92, 144);
            this.dataGrv_inp_B.TabIndex = 16;
            this.dataGrv_inp_B.Visible = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(286, 197);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(135, 20);
            this.label6.TabIndex = 17;
            this.label6.Text = "Правый столбец";
            this.label6.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(290, 278);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 31);
            this.label7.TabIndex = 18;
            this.label7.Text = "=";
            this.label7.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 436);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dataGrv_inp_B);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dataGrv_simplex_table);
            this.Controls.Add(this.btn_calculate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGrv_inp_function);
            this.Controls.Add(this.lbl_inp_matrix);
            this.Controls.Add(this.btn_repeat);
            this.Controls.Add(this.txtb_m);
            this.Controls.Add(this.txtb_n);
            this.Controls.Add(this.dataGrv_inp_matrix);
            this.Controls.Add(this.lbl_m);
            this.Controls.Add(this.lbl_n);
            this.Controls.Add(this.btn_generate);
            this.Name = "Form1";
            this.Opacity = 0.9D;
            this.ShowIcon = false;
            this.Text = "Решение матричной игры симплекс методом для второго игрока";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrv_inp_matrix)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrv_inp_function)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrv_simplex_table)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrv_inp_B)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_generate;
        private System.Windows.Forms.Label lbl_n;
        private System.Windows.Forms.Label lbl_m;
        private System.Windows.Forms.TextBox txtb_n;
        private System.Windows.Forms.TextBox txtb_m;
        private System.Windows.Forms.Button btn_repeat;
        public System.Windows.Forms.DataGridView dataGrv_inp_matrix;
        private System.Windows.Forms.Label lbl_inp_matrix;
        public System.Windows.Forms.DataGridView dataGrv_inp_function;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_calculate;
        public System.Windows.Forms.DataGridView dataGrv_simplex_table;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        public System.Windows.Forms.DataGridView dataGrv_inp_B;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}

