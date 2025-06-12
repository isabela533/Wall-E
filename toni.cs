<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:Wall_E"
        mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
        x:Class="Visual.MainWindow"
        Title="Visual">
    <Grid x:Name="MainGrid" Background="White">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*"/>
            <ColumnDefinition Width="2*"/>
            <ColumnDefinition Width="2*"/>
            <ColumnDefinition Width="*"/>
        </Grid.ColumnDefinitions>

        <Grid.RowDefinitions>
            <RowDefinition Height="0.5*"/>
            <RowDefinition Height="5*"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <Rectangle x:Name="First" Fill="Aqua" Grid.RowSpan="2"/>
        <Rectangle Fill="Aquamarine" Grid.Column="1"/>
        <Border 
            BorderBrush="Black" 
            BorderThickness="2">
            <Rectangle Fill="Aqua"/>
        </Border>

        <ScrollViewer Grid.Column="1" Grid.ColumnSpan="2" Grid.Row="1"
            VerticalScrollBarVisibility="Visible"
            HorizontalScrollBarVisibility="Visible">
            <TextBox AcceptsReturn="True" AcceptsTab="True"
                BorderBrush="Red" BorderThickness="3"/>
        </ScrollViewer>
    </Grid>
</Window>