using RussianPost.Classes;
using RussianPost.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using RussianPost.Helpers;
using ZXing;

// Документацию по шаблону элемента "Пустая страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=391641

namespace RussianPost
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Config config;
        List<PackageMin> Packages;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            config = new Config();
            config.Init();

            this.ReloadPackages();
            ProgressIndicator.Opacity = 0;
        }

        private void ReloadPackages()
        {
            Packages = config.GetPackages();
            packageList.ItemsSource = null;
            packageList.ItemsSource = Packages;

            /*
             * Внимание, вопрос! Нахуя нужно присваивать сначала нуль, а потом уже массив с данными?
             * 
             * Ответ: А потому-что иначе при обновлении списка приложение крашается, и чтобы это избежать, приходится
             * заниматься вот такой эквилибристикой. 
             * 
             * Как бы сейчас сказал Изя: Потому-что п...
             * Ну или Кластер: Потому-что китайцы. Хотя вроде Си Шарп в Рэдмонде придумали.
             */
        }

        /*
         * Отличие от ReloadPackages() заключается в том, что эта функция обновляет информацию о посылках, а не просто 
         * перезагружает список
         * 
         */
        private async void ReloadPackagesWithUpdate()
        {
            ProgressIndicator.Opacity = 1;
            Packages = config.GetPackages(); // На всякий случай

            AddButton.IsEnabled = false;
            ReloadButton.IsEnabled = false;

            List<PackageMin> newList = new List<PackageMin>();
            int failed = 0;
            int count = 0;
            foreach (PackageMin pkg in Packages)
            {
                count++;
                TitleLabel.Text = "ПОЧТА СТРАНЫ | осталось обновить: " + (Packages.Count - count);

                Package UpdatedPackage = null;
                try
                {
                    string TrackID = pkg.ID;
                    var API = new API();
                    UpdatedPackage = await API.getPackageByID(TrackID);
                } catch (System.Exception ex)
                {
                    failed++;
                }

                PackageMin NewPackage = new PackageMin();
                NewPackage.ID = UpdatedPackage.ID;
                NewPackage.Name = UpdatedPackage.Name;
                NewPackage.LastState = UpdatedPackage.CommonStatus;
                NewPackage.ReadyToGet = false;

                newList.Add(NewPackage);
            }
            Packages = newList;
            config.ImportNewList(newList);
            config.Save();
            this.ReloadPackages();
            ProgressIndicator.Opacity = 0;
            TitleLabel.Text = "ПОЧТА СТРАНЫ";
            AddButton.IsEnabled = true;
            ReloadButton.IsEnabled = true;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            AddPackageDialog addPackage = new AddPackageDialog();
            await addPackage.ShowAsync();

            if (addPackage.Result == ContentDialogResult.Primary)
            {
                ProgressIndicator.Opacity = 1;

                try
                {
                    var API = new API();
                    var Package = await API.getPackageByID(addPackage.TrackID);
                    
                    config.AddPackage(addPackage.TrackID,
                                            Package.Name,
                                            Package.CommonStatus, 
                                            false);
                    config.Save();
                    this.ReloadPackages();
                    this.Frame.Navigate(typeof(PackagePage), Package);
                } catch (Exception ex)
                {
                    var dialog = new MessageDialog(ex.Message, "Что-то пошло не так.");
                    await dialog.ShowAsync();
                    System.Diagnostics.Debug.WriteLine(ex);
                }

                ProgressIndicator.Opacity = 0;
            }
        }

        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            MessageDialog About = new MessageDialog("Почта Страны. Версия 1.0.0. Автор: @veselcraft", "О ПРОГРАММЕ");
            await About.ShowAsync();
        }

        private void Item_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            PackageMin whichOne = senderElement.DataContext as PackageMin;
            FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            flyoutBase.ShowAt(senderElement);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            PackageMin datacontext = (e.OriginalSource as FrameworkElement).DataContext as PackageMin;
            config.RemovePackage(datacontext.ID);
            this.ReloadPackages();
            config.Save();
        }

        private async void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ProgressIndicator.Opacity = 1;

            FrameworkElement senderElement = sender as FrameworkElement;
            PackageMin whichOne = senderElement.DataContext as PackageMin;

            var API = new API();
            var Package = await API.getPackageByID(whichOne.ID);

            this.Frame.Navigate(typeof(PackagePage), Package);

            ProgressIndicator.Opacity = 0;
        }

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            this.ReloadPackagesWithUpdate();
        }
    }
}
