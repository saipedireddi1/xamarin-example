using MapKit;

namespace CustomRenderer.iOS
{
	public class CustomMKAnnotationView : MKAnnotationView
	{
		public string Id { get; internal set; }
		public bool isCustom { get; set; }

		public CustomMKAnnotationView(IMKAnnotation annotation, string id)
			: base(annotation, id)
		{
		}
	}
}