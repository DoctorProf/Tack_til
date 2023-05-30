using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tack_til.Data;
using Tack_til.Models;
using Tack_til.ViewModels.Base;


namespace Tack_til.ViewModels
{
    internal class GameViewModel : ViewModel
    {
        private FieldModel selectedField = null;
        public FieldModel SelectedField { get => selectedField; set => Set(ref selectedField, value); }
        private Color.Piece move = Color.Piece.White;
        public Color.Piece Move { get => move; set => Set(ref move, value); }

        #region Field Observable Collection
        private ObservableCollection<ObservableCollection<FieldModel>> f = new();
        public ObservableCollection<ObservableCollection<FieldModel>> F { get => f; set => Set(ref f, value); }
        #endregion

        #region Constructor
        public GameViewModel()
        {
            StartGame();
        }
        #endregion

        #region Methods

        #region StartGame
        public void StartGame()
        {
            Move = Color.Piece.White;
            GenerateField();
            StartingPosition();
        }
        #endregion
        #region SetPointCheck
        public void SetPointCheck(int checkposi, int checkposj)
        {
            if (CheckOnBoard(checkposi, checkposj))
            {
                if (SelectedField.PieceColor == Color.Piece.White)
                {
                    if (F[checkposi][checkposj].TexturePath == ImagesConstsPaths.Empty)
                    {
                        SetPoint(checkposi, checkposj);
                    }
                }
                if (SelectedField.PieceColor == Color.Piece.Black)
                {
                    if (F[checkposi][checkposj].TexturePath == ImagesConstsPaths.Empty)
                    {
                        SetPoint(checkposi, checkposj);
                    }
                }
            }
        }
        #region Walk
        public void Walk()
        {
            int checkposi = SelectedField.I;
            int checkposi1 = SelectedField.I + 1;
            int checkposi2 = SelectedField.I - 1;

            int checkposj = SelectedField.J;
            int checkposj1 = SelectedField.J + 1;
            int checkposj2 = SelectedField.J - 1;

            SetPointCheck(checkposi, checkposj1);
            SetPointCheck(checkposi, checkposj2);
            SetPointCheck(checkposi1, checkposj);
            SetPointCheck(checkposi2, checkposj);


        }
        #endregion
        #endregion
        #region CheckOnBoard
        public static bool CheckOnBoard(int i, int j)
        {
            return i >= 0 && i < 8 && j >= 0 && j < 8;
        }
        #endregion

        #region Quit

        public static void Quit() { Application.Current.Shutdown(); }

        #endregion

        #region RestartGame

        public void RestartGame(string text)
        {
            MessageBoxResult mbr = MessageBox.Show($"{text} Начать заново?", "Конец игры", MessageBoxButton.YesNo);
            if (mbr == MessageBoxResult.Yes) StartGame();
            else Quit();
        }

        #endregion

        #region Win

        public void Win(Color.Piece color)
        {
            string colorName = (color == Color.Piece.White) ? "белые" : "черные";
            RestartGame($"Выйграли {colorName}.");
        }

        #endregion

        #region CheckWin
        public void CheckWin()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    FieldModel fm = F[i][j];
                    if (fm.PieceColor == Color.Piece.Empty) continue;

                    int count;

                    // проверяем вертикально вниз
                    count = 0;
                    for (int a = 0; a < 4; a++)
                    {
                        if (CheckOnBoard(i + a, j) && F[i + a][j].PieceColor == fm.PieceColor)
                        {
                            count++;
                        }
                    }
                    if (count == 4)
                    {
                        Win(fm.PieceColor);
                    }

                    // проверяем по горизонтали вправо
                    count = 0;
                    for (int a = 0; a < 4; a++)
                    {
                        if (CheckOnBoard(i, j + a) && f[i][j + a].PieceColor == fm.PieceColor)
                        {
                            count++;
                        }
                    }
                    if (count == 4)
                    {
                        Win(fm.PieceColor);
                    }

                }
            }
        }
        #endregion

        public void ReverseFigures(FieldModel field)
        {

            field.PieceColor = SelectedField.PieceColor;
            SelectedField.PieceColor = Color.Piece.Empty;
            SelectedField.Selected = false;
            SelectedField = null;
            ClearPoints();
            Move = Move == Color.Piece.White ? Color.Piece.Black : Color.Piece.White;
            CheckWin();
        }
        public void SetPoint(int i, int j)
        {
            if (((i >= 0 & i <= 7) & (j >= 0 & j <= 7)) && F[i][j].TexturePath == ImagesConstsPaths.Empty)
            {
                F[i][j].TexturePath = ImagesConstsPaths.Point;
            }
        }
        public void ClearPoints()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (F[i][j].TexturePath == ImagesConstsPaths.Point)
                    {
                        F[i][j].TexturePath = ImagesConstsPaths.Empty;
                    }
                }
            }
        }
        public void Click(FieldModel field)
        {
            if (field.TexturePath != ImagesConstsPaths.Empty && field.PieceColor == Move)
            {
                if (field.PieceColor == Move)
                {

                    if (SelectedField == null)
                    {
                        SelectedField = field;
                        SelectedField.Selected = true;
                        Walk();
                    }
                    else
                    {
                        if (field == SelectedField)
                        {
                            SelectedField.Selected = false;
                            SelectedField = null;
                            ClearPoints();
                            
                        }
                        else if (field != SelectedField)
                        {
                            SelectedField.Selected = false;
                            ClearPoints();
                            SelectedField = field;
                            SelectedField.Selected = true;
                            Walk();
                        }
                    }
                }
            }
            else
            {
                if (SelectedField != null)
                {
                    if (field.TexturePath == ImagesConstsPaths.Point)
                    {
                            ReverseFigures(field);
                    }
                }
            }
        }

        #region GenerateField
        public void GenerateField()
        {
            f.Clear();
            for (int i = 0; i < 8; i++)
            {
                ObservableCollection<FieldModel> row = new();
                for (int j = 0; j < 8; j++)
                {
                    row.Add(new FieldModel() { I = i, J = j, BackgroundColor = (i + j) % 2 != 0 ? "#e2b87e" : "#712609" });
                }
                f.Add(row);
            }
        }
        #endregion

        #endregion
        #region Starting position
        public void StartingPosition()
        {
            for(int i = 0;i < 1;i++) 
            {
                for(int j = 1;j < 7;j++) 
                { 
                    if(j % 2 !=0)
                    {
                        F[i][j].PieceColor = Color.Piece.Black;
                    } else
                    {
                        F[i][j].PieceColor = Color.Piece.White;
                    }
                }
            }
            for (int i = 7; i < 8; i++)
            {
                for (int j = 1; j < 7; j++)
                {
                    if (j % 2 != 0)
                    {
                        F[i][j].PieceColor = Color.Piece.White;
                    }
                    else
                    {
                        F[i][j].PieceColor = Color.Piece.Black;
                    }
                }
            }
        }

        #endregion
    }
}
