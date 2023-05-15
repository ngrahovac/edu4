import React, { useEffect } from 'react'
import { useState } from 'react';
import HatForm from '../comps/hat-forms/HatForm';
import AddedPosition from '../comps/hats2/AddedPosition';
import { DoubleColumnLayout } from '../layout/DoubleColumnLayout'
import { SectionTitle } from '../layout/SectionTitle'
import SubsectionTitle from '../layout/SubsectionTitle';
import NeutralButton from '../comps/buttons/NeutralButton';
import PrimaryButton from '../comps/buttons/PrimaryButton';
import { addPositions, updateDetails } from '../services/ProjectsService';
import {
    successResult,
    failureResult,
    errorResult
} from '../services/RequestResult'
import { useAuth0 } from '@auth0/auth0-react'
import PositionCard from '../comps/discover/PositionCard';


const EditProject = () => {
    const [selectedPositionType, setSelectedPositionType] = useState("Student");

    const [position, setPosition] = useState({});

    const [project, setProject] = useState({
        "id": "74c6895a-1fdd-4149-aeda-f3c71d3a07db",
        "datePosted": "2023-03-31T08:15:37.684Z",
        "title": "Mobile App Development",
        "description": "We are looking for a team of developers to create a mobile app for our company.",
        "authorId": "ce075dea-7706-409e-91e8-7f27580d2da0",
        "authored": true,
        "recommended": false,
        "positions": [
            {
                "id": "aa454375-2469-46c5-83ee-aaee3ad2ee0e",
                "datePosted": "2023-04-14T22:53:55.0701872Z",
                "name": "Android Developer",
                "description": "Responsible for developing and maintaining the Android version of the app.",
                "requirements": {
                    "type": "Student",
                    "parameters": {
                        "type": 0,
                        "studyField": "Computer Science",
                        "academicDegree": 1
                    }
                },
                "recommended": true
            },
            {
                "id": "c76ec284-ed62-4099-b1ed-fc0bef743def",
                "datePosted": "2023-04-14T22:53:55.0702114Z",
                "name": "iOS Developer",
                "description": "Responsible for developing and maintaining the iOS version of the app.",
                "requirements": {
                    "type": "Student",
                    "parameters": {
                        "type": 0,
                        "studyField": "Computer Science",
                        "academicDegree": 2
                    }
                },
                "recommended": true
            },
            {
                "id": "c76ec284-ed62-4099-b1ed-fc0bef743def",
                "datePosted": "2023-04-14T22:53:55.0702114Z",
                "name": "iOS Developer",
                "description": "Not much",
                "requirements": {
                    "type": "Student",
                    "parameters": {
                        "type": 0,
                        "studyField": "Electrical engineering",
                        "academicDegree": 2
                    }
                },
                "recommended": false
            }
        ]
    });

    const [newPositions, setNewPositions] = useState([]);

    const { getAccessTokenSilently, getAccessTokenWithPopup } = useAuth0();

    function onBasicInfoFormChange(e) {
        setProject({ ...project, [e.target.name]: e.target.value })
    }

    function onPositionFormChange(e) {
        if (e.target.name == "positionType") {
            setSelectedPositionType(e.target.value);
        } else {
            setPosition({ ...position, [e.target.name]: e.target.value });
        }
    }

    function setPositionRequirements(hat) {
        setPosition({ ...position, requirements: hat });
    }

    function removeNewPosition(positionToRemove) {
        let filteredPositions = newPositions.filter(p => p != positionToRemove);
        setNewPositions(filteredPositions);
    }

    function onAddPositions() {
        (async () => {
            try {
                {/* add validation */ }
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await addPositions(project.id, newPositions, token);

                if (result.outcome === successResult) {
                    console.log("success");
                    // document.getElementById('user-action-success-toast').show();
                    // setTimeout(() => window.location.href = "/homepage", 1000);
                } else if (result.outcome === failureResult) {
                    console.log("failure");
                    // document.getElementById('user-action-fail-toast').show();
                    // setTimeout(() => {
                    //     document.getElementById('user-action-fail-toast').close();
                    // }, 3000);
                } else if (result.outcome === errorResult) {
                    console.log("network error");
                    // document.getElementById('user-action-fail-toast').show();
                    // setTimeout(() => {
                    //     document.getElementById('user-action-fail-toast').close();
                    // }, 3000);
                }
            } catch (ex) {
                console.log("error");
                // document.getElementById('user-action-fail-toast').show();
                // setTimeout(() => {
                //     document.getElementById('user-action-fail-toast').close();
                // }, 3000);
            }
        })();
    }

    function onUpdateDetails() {
        (async () => {
            try {
                {/* add validation */ }
                let token = await getAccessTokenWithPopup({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await updateDetails(project.id, project.title, project.description, token);

                if (result.outcome === successResult) {
                    console.log("success");
                    // document.getElementById('user-action-success-toast').show();
                    // setTimeout(() => window.location.href = "/homepage", 1000);
                } else if (result.outcome === failureResult) {
                    console.log("failure");
                    // document.getElementById('user-action-fail-toast').show();
                    // setTimeout(() => {
                    //     document.getElementById('user-action-fail-toast').close();
                    // }, 3000);
                } else if (result.outcome === errorResult) {
                    console.log("network error");
                    // document.getElementById('user-action-fail-toast').show();
                    // setTimeout(() => {
                    //     document.getElementById('user-action-fail-toast').close();
                    // }, 3000);
                }
            } catch (ex) {
                console.log("error");
                // document.getElementById('user-action-fail-toast').show();
                // setTimeout(() => {
                //     document.getElementById('user-action-fail-toast').close();
                // }, 3000);
            }
        })();
    }

    const left = (
        <div className='relative pb-32'>
            <div className='mb-12'>
                <SectionTitle title="Basic info"></SectionTitle>
            </div>
            <form
                onChange={onBasicInfoFormChange}>
                <div className='mb-8'>
                    <label>
                        <p>Title*</p>
                        <input
                            type="text"
                            name="title"
                            value={project.title}
                            className="w-full mt-1 block rounded-md border-gray-300 focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10"></input>
                    </label>
                </div>

                <div className='mb-8'>
                    <label>
                        <p>Description*</p>
                        <textarea
                            rows={5}
                            maxLength={1000}
                            name="description"
                            value={project.description}
                            className="resize-y mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10"></textarea>
                    </label>
                </div>

                <div className='flex flex-row content-between justify-between'>
                    <label>
                        <p>Start date*</p>
                        <input
                            type="date"
                            name="startDate"
                            className="mt-1 w-64 block rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10">
                        </input>
                    </label>

                    <label>
                        <p>End date*</p>
                        <input
                            type="date"
                            name="endDate"
                            className="mt-1 w-64 block rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10">
                        </input>
                    </label>
                </div>
            </form>

            <div className='flex flex-row shrink-0 absolute bottom-2 right-0 space-x-2'>
                <NeutralButton
                    text="Cancel"
                    onClick={() => { }}>
                </NeutralButton>

                <PrimaryButton
                    text="Update details"
                    onClick={onUpdateDetails}>
                </PrimaryButton>
            </div>
        </div>
    );

    const right = (
        <div className='relative pb-32'>
            <div className='mb-2'>
                <div className="mb-8">
                    <SectionTitle title="Positions"></SectionTitle>
                    <p>Describe the profiles of people you're looking to find and collaborate with</p>
                </div>

                <SubsectionTitle title="Existing positions"></SubsectionTitle>
            </div>
            {
                project.positions.length == 0 &&
                <p className='text-gray-500'>Currently there are no added positions.</p>
            }
            {
                project.positions.length > 0 &&
                <div className="mt-4">
                    {
                        project.positions.map(p => (
                            <div key={Math.random() * 1000}>
                                <div className='mb-2'>
                                    {/*  <Position position={p}></Position> */}
                                    <PositionCard
                                        position={p}>
                                    </PositionCard>
                                </div>
                            </div>)
                        )
                    }
                </div>
            }

            <form
                onChange={onPositionFormChange}>
                <div className='mb-8 mt-16'>
                    <div className='mb-2'>
                        <SubsectionTitle title="Add a position"></SubsectionTitle>
                    </div>
                    <label>
                        <p>Title*</p>
                        <input
                            type="text"
                            name="name"
                            value={position.name}
                            placeholder='e.g. .NET Backend Developer'
                            className="w-full mt-1 block rounded-md border-gray-300 focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10"></input>
                    </label>
                </div>

                <div className='mb-8'>
                    <label>
                        <p>Description*</p>
                        <textarea
                            name="description"
                            rows={5}
                            maxLength={1000}
                            value={position.description}
                            className="resize-y mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10"></textarea>
                    </label>
                </div>

                <div className='mb-8'>
                    <label>
                        <p>Type*</p>
                        <select
                            name="positionType"
                            value={selectedPositionType}
                            className="block w-full mt-1 rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10">
                            <option name="Student">Student</option>
                            <option name="Academic">Academic</option>
                        </select>
                    </label>
                </div>
            </form>

            <div
                className='relative pb-16 mb-16'>
                <HatForm
                    hatType={selectedPositionType}
                    onHatChanged={setPositionRequirements}>
                </HatForm>

                <div className='absolute bottom-2 right-0'>
                    <NeutralButton
                        text="Add"
                        onClick={() => {
                            setNewPositions([...newPositions, position])
                        }}>
                    </NeutralButton>
                </div>
            </div>



            <div className='mb-4 mt-12'>
                <SubsectionTitle title="New positions"></SubsectionTitle>
            </div>
            {
                newPositions.length == 0 &&
                <p className='text-gray-500'>Currently there are no added positions.</p>
            }

            {
                newPositions.map(p => (
                    <div key={Math.random() * 1000}>
                        <div className='mb-2'>
                            {/*  <Position position={p}></Position> */}
                            <AddedPosition
                                position={p}
                                onRemoved={() => removeNewPosition(p)}>
                            </AddedPosition>
                        </div>
                    </div>)
                )
            }

            <div className='flex flex-row shrink-0 absolute bottom-2 right-0 space-x-2'>
                <NeutralButton
                    text="Cancel"
                    onClick={() => { }}>
                </NeutralButton>

                <PrimaryButton
                    text="Update positions"
                    onClick={onAddPositions}>
                </PrimaryButton>
            </div>
        </div>
    );

    return (
        <DoubleColumnLayout
            title="Edit project"
            description=""
            left={left}
            right={right}>
        </DoubleColumnLayout>
    );
}

export default EditProject