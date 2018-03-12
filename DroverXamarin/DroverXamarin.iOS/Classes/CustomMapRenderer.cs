using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CoreGraphics;
using CustomRenderer.iOS;
using DroverXamarin;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace CustomRenderer.iOS
{
	public class CustomMapRenderer : MapRenderer
	{
		UIView customPinView;
		private ObservableCollection<CustomPin> customPins;

		public ObservableCollection<CustomPin> CustomPins
		{
			get
			{
				if (customPins == null)
				{
					customPins = new ObservableCollection<CustomPin>();
				}
				return customPins;
			}
			set
			{
				customPins = value;
			}
		}

		protected override void OnElementChanged(ElementChangedEventArgs<View> e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null)
			{
				var nativeMap = Control as MKMapView;
				nativeMap.GetViewForAnnotation = null;
			}

			if (e.NewElement != null)
			{
				var formsMap = (CustomMap)e.NewElement;
				var nativeMap = Control as MKMapView;
				customPins = (ObservableCollection<CustomPin>)formsMap.CustomPins;

				nativeMap.GetViewForAnnotation = GetViewForAnnotation;

				var annos = nativeMap.Annotations;
				nativeMap.RemoveAnnotations();
				nativeMap.AddAnnotations(annos);
			}

			MessagingCenter.Subscribe<MainPage>(this, "updated pins", (Sender) =>
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					PinUpdated(e);
				});
			});
		}

		private void PinUpdated(ElementChangedEventArgs<View> e)
		{
			var formsMap = (CustomMap)e.NewElement;
			var nativeMap = Control as MKMapView;
			customPins = formsMap.CustomPins;

			if (nativeMap != null && nativeMap.Annotations != null && nativeMap.Annotations.Count() > 0)
			{
				foreach (var a in nativeMap.Annotations)
				{
					var title = a.GetTitle();
					if (!string.IsNullOrEmpty(title))
					{

					}
					var pin = customPins.FirstOrDefault(p => p.Pin.Label == title);
					if (pin != null)
					{
						a.SetCoordinate(new CoreLocation.CLLocationCoordinate2D
						{
							Latitude = pin.Pin.Position.Latitude,
							Longitude = pin.Pin.Position.Longitude
						});
					}
				}
				nativeMap.SetNeedsDisplay();
			}
		}

		MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
		{
			MKAnnotationView annotationView = null;

			if (annotation == null)
			{
				return annotationView;
			}

			if (annotation is MKUserLocation)
				return null;

			var anno = annotation as MKPointAnnotation;
			if (anno == null)
			{
				return null;
			}
			var customPin = GetCustomPin(anno);
			if (customPin == null)
			{
				throw new Exception("Custom pin not found");
			}

			annotationView = mapView.DequeueReusableAnnotation(customPin.Id);
			if (annotationView == null)
			{
				annotationView = new CustomMKAnnotationView(annotation, customPin.Id);
				if (customPin.Id == "Driver")
				{
					annotationView.Image = UIImage.FromFile("driver_pin.png");
				}
				else if (customPin.Id == "Pick Up Location")
				{
					annotationView.Image = UIImage.FromFile("pick_up_pin.png");
				}
				else {
					annotationView.Image = UIImage.FromFile("drop_off_pin.png");
				}

				((CustomMKAnnotationView)annotationView).Id = customPin.Id;
				((CustomMKAnnotationView)annotationView).isCustom = customPin.isCustom;
			}
			annotationView.CanShowCallout = true;

			return annotationView;
		}

		CustomPin GetCustomPin(MKPointAnnotation annotation)
		{
			var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
			foreach (var pin in customPins)
			{
				if (pin.Pin.Position == position)
				{
					return pin;
				}
			}

			return null;
		}
	}
}