   M S B A M L   `   `   `  ����G  CMarkLandmark, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:MarkLandmarkMarkLandmark  5   	       MarkLandmark.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   c _System.Windows.Interactivity, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5   	   &localclr-namespace:MarkLandmark  5   	   d `GalaSoft.MvvmLight.Extras, Version=5.3.0.19026, Culture=neutral, PublicKeyToken=f46ff315b1088208+mvvm http://www.galasoft.ch/mvvmlight  5   	     � WindowStyle$  SingleBorderWindow=�5
   	    � 
ResizeMode$ NoResize=�5   	    � WindowStartupLocation$ CenterScreen=�5   	    � WindowState$ 	Maximized=�5   	   ���#111111 � Title 5   	   �� *	ImagePath+ � 	Resources 5      /  )MarkLandmark.LandmarkNameToColorConverter   LandmarkColorConverter   FaceContourColor   EyebrowColor   EyeColor	   	NoseColor
   
MouthColor   DefaultColor6  0MarkLandmark.LandmarkNameToVisualStringConverter ! LandmarkVisualStringConverter��%�   &	        &	 Y       5   
   
 � � �
 �� ��
 � E��
	 ����

 ��  �
 �  ��5   -     5   
   64   5      .  (System.Windows.Interactivity.Interaction   Triggers 5      /  )System.Windows.Interactivity.EventTrigger  5   
      	EventName$ KeyDown��6   .  (System.Windows.Interactivity.TriggerBase   Actions. 5      /  )GalaSoft.MvvmLight.Command.EventToCommand     PassEventArgsToCommand .    Command 6"   ��  �� Path$ 
KeyDownCmd�5   "   5        5   
   $ Loaded��6   . 5          6"   �� $ 	WndLoaded�5!        5"   
   $ Closed��6   . 5#         6"   �� $ 	WndClosed�5$       � DataContext 5&          MarkLandmark.MainViewModel  5'   
   .��5)      �  � RowDefinitions 5*   
   �� 5+      $
��Auto �6   �� 5,      .+�5.   
   ]� .+�5/      o� $	��Top=�6   .@�50      m�   MenuItem_File_Header#��[ 6   .@�51      m�   MenuItem_Open_Header#��[ 6    m� Command 6N   �� *OpenDatasetCmd+m� 52        MenuItem_Save_Header#��[ 6    6N   �� *	SaveCmd+{�6j   �� *IsSaveEnabled+m� 53       ! MenuItem_PreviousImage_Header#��[ 6    6W   �� *PreviousCmd+{�6w   �� *IsPreviousEnabled+m� 54        MenuItem_NextImage_Header#��[ 6    6S   �� *	NextCmd+{�6o   �� *IsNextEnabled+m� 55       " MenuItem_PreviousFolder_Header#��[ 6    6X   �� *PreviousFolderCmd+{�6~   �� *IsPreviousFolderEnabled+m� 56        MenuItem_NextFolder_Header#��[ 6    6T   �� *NextFolderCmd+{�6v   �� *IsNextFolderEnabled+m� 57       	 MenuItem_Close_Header#��[	 6    6O   �� *
CloseCmd+58      59      �� 5:      ���#1111116   5;      � 5=   
   $��1��6      MarkLandmark.DataPiping   	DataPipes 5?      %	   MarkLandmark.DataPipeCollection	  
   MarkLandmark.DataPipe
  5@       
  Source 6%   ��  �� RelativeSource ��  �� AncestorType" �$ ActualWidth� 
  Target 5A   %   �� $ ImagePanelWidth� �� Mode$ OneWayToSource=�
  5B       6%   ��  �� " �$ ActualHeight� 5C   %   �� $ ImagePanelHeight�$ OneWayToSource=�5D      .+�5F      �� $��Uniform=�5G      ��5F      �� $ ImageSource�$ OneWay=�5G      �� 5I      ��6   �� $ RenderedLandmarks���5J      �� 5K      .R�5L      �� 5M      ��5O      �� 5P      .R�5Q      A� $��little_landmark��6"   $��8��5R   "   $��8��6,    J� RenderTransformOrigin$ 0.5,0.5&�5T   "    � Cursor$ Cross��5W   "   ��5S   "   �� $
 Name� �� 	Converter# [   � ToolTip 5U   "   �� $
 Name�# [ y�5V   "   �� $ 
Visibility� 5X        5Y   "   $ MouseLeftButtonDown��61   . 5Z   &      .  6:   �� $ MouseLeftButtonDownCmd�5[   :   5\   #     5]   "   $ Loaded��61   . 5^   &   6  0System.Windows.Interactivity.InvokeCommandAction     Command 6<   �� $ 	LoadedCmd�    CommandParameter  5_   <   �� ! �� ElementName$! little_landmark��5`   #   z�5b      T� 5c   "   " T� X" 65   �� *X+$ TwoWay=�# T� Y# 6R   �� *Y+$ TwoWay=�5e      5f      5h      5i      5j      5k      