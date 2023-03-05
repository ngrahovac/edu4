import React from 'react'
import { useState } from 'react';
import HatForm from '../comps/hat-forms/HatForm';
import AddedPosition from '../comps/hats2/AddedPosition';
import { DoubleColumnLayout } from '../layout/DoubleColumnLayout'
import { SectionTitle } from '../layout/SectionTitle'
import SubsectionTitle from '../layout/SubsectionTitle';
import NeutralButton from '../comps/buttons/NeutralButton';
import PrimaryButton from '../comps/buttons/PrimaryButton';

const Publish = () => {
    {/* TODO: remove static data */ }

    const [selectedPositionType, setSelectedPositionType] = useState("Student");

    const [position, setPosition] = useState({});

    const [project, setProject] = useState({ positions: [] });

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

    function removePosition(positionToRemove) {
        let filteredPositions = project.positions.filter(p => p != positionToRemove);
        console.log(filteredPositions)
        setProject({ ...project, positions: filteredPositions });
    }

    function onPublishProject() {
        console.log("publishing", project);
    }

    const left = (
        <>
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
        </>
    );

    const right = (
        <div className='relative pb-32'>
            <form
                onChange={onPositionFormChange}>
                <div className="mb-8">
                    <SectionTitle title="Positions"></SectionTitle>
                    <p>Describe the profiles of people you're looking to find and collaborate with</p>
                </div>

                <div className='mb-8'>
                    <label>
                        <p>Title*</p>
                        <input
                            type="text"
                            name="title"
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
                            className="resize-y mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-600 focus:ring focus:ring-indigo-200 focus:ring-opacity-10"></textarea>
                    </label>
                </div>

                <div className='mb-8'>
                    <label>
                        <p>Type*</p>
                        <select
                            name="positionType"
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
                            setProject({ ...project, positions: [...project.positions, position] })
                        }}>
                    </NeutralButton>
                </div>
            </div>

            <div className='mb-2'>
                <SubsectionTitle title="Added positions"></SubsectionTitle>
            </div>
            {
                project.positions.length == 0 &&
                <p className='text-gray-500'>Currently there are no added positions.</p>
            }
            {
                project.positions.length > 0 &&
                <div className="mt-4"> {
                    project.positions.map(p => (
                        <div key={Math.random() * 1000}>
                            <div className='mb-2'>
                                {/*  <Position position={p}></Position> */}
                                <AddedPosition
                                    position={p}
                                    onRemoved={() => removePosition(p)}>
                                </AddedPosition>
                            </div>
                        </div>)
                    )
                }
                </div>
            }

            <div className='absolute bottom-2 right-0'>
                <PrimaryButton
                    text="Publish"
                    onClick={onPublishProject}>
                </PrimaryButton>
            </div>
        </div>
    );

    return (
        <DoubleColumnLayout
            title="Publish a project"
            description="Describe the project or an idea you're working on, let the world know about it and find your people."
            left={left}
            right={right}>
        </DoubleColumnLayout>
    );
}

export default Publish