import React, { useCallback } from 'react';
import { GoogleMap, Marker, useLoadScript } from '@react-google-maps/api';

const mapKey = process.env.REACT_APP_MAPS_API_KEY;
const libraries = ['places'];
const mapContainerStyle = {
  height: '250px',
};
const options = {
  disableDefaultUI: true,
  gestureHandling: 'none',
  keyboardShortcuts: false,
};

const MapImage = ({ latitude, longitude }) => {
  const { isLoaded, loadError } = useLoadScript({
    googleMapsApiKey: mapKey,
    libraries: libraries,
  });

  const noDrag = useCallback(() => {
    return false;
  }, []);

  const center = {
    lat: latitude,
    lng: longitude,
  };

  if (loadError) return
  if (!isLoaded) return
  
  return (
    <div data-testid="mapimagecomponent">
      <GoogleMap mapContainerStyle={mapContainerStyle} center={center} zoom={14} options={options} onDrag={noDrag}>
        <Marker position={{ lat: latitude, lng: longitude }} />
      </GoogleMap>
    </div>
  );
};

export default MapImage;
