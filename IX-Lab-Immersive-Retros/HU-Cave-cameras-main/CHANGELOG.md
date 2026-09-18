# Changelog

## [0.3.9]

### Changed
- Changed Cave center into Cave offset.

### Fixed
- Check for camera in the cave cameras on awake > needed if serialization doesn't save.

## [0.3.7 - 0.3.8]

### Fixed
- Check for camera setup on start as well.

## [0.3.6]

### Fixed
- Flipping of vertical axis is now specific to reversed-z buffering.

## [0.3.5]

### Fixed
- Projection now keeps reversed-Z buffering in mind.

## [0.3.4]

### Fixed
- Camera's now correctly use 0,0,0 as projection calculation.

## [0.3.3]

### Fixed
- Changelog header of 0.3.2
- UI no longer fully covers left side of display
- Made CameraManager's validation rules more accurate
- Changed Cave size calibration to make sense in the room
- Fixed Text coloring
- Fixed Frustum calculations in relation to changed cave size.

## [0.3.2]

### Fixed
- Fixed CameraManager's validation rules

## [0.3.1]

### Fixed
- Camera's not being loaded outside of play for editor calibration.
- Camera not making use of the local center.
- Readme meta not included in package

## [0.3.0]

### Added
- swapping of camera outputs.

### Fixed
- Calibration UI improved.

## [0.2.2]

## Fixed
- Put center in local transform

## [0.2.1]

### Fixed
- Eye height misplacing the projection

## [0.2.0]

### Added
- Cave game window setup 

## [0.1.4]

### Fixed
- Fixed CaveCameraManager having two start functions during build.

## [0.1.3]

### Fixed
- Fixed default style sheet not transfered

## [0.1.2]

### Fixed
- Fixed default visual tree diagram used.

## [0.1.1]

### Fixed
- Fixed EditorDialog not existing before unity version 6.3

## [0.1.0]

### Added 
- This Changelog
- package manifest
- Cave Camera setup
- Cave Data saving
- Manual calibration UI
