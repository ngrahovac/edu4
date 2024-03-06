import React from 'react'
import HatParam from './HatParam'

const AcademicHat = (props) => {
    const {
        hat,
        ownHats = undefined
    } = props;

    let typeMatch = ownHats?.find(h => h.type == "Academic") != undefined;
    let fieldMatch = ownHats?.find(h => h.parameters.researchField == hat.parameters.researchField || h.parameters.studyField == hat.parameters.researchField);
    

    return (
        <div className='flex gap-x-2 gap-y-1 flex-wrap'>
            <HatParam match={typeMatch}>Academic / Researcher</HatParam>
            <HatParam match={fieldMatch}>{hat.parameters.researchField}</HatParam>
        </div>
    )
}

export default AcademicHat