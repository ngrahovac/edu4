import React, { useRef } from 'react'
import { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import SpinnerLayout from '../layout/SpinnerLayout';
import { BeatLoader } from 'react-spinners';
import { useAuth0 } from '@auth0/auth0-react';
import ProjectCard from '../comps/discover/ProjectCard';
import SingleColumnLayout from '../layout/SingleColumnLayout';
import { me } from '../services/UsersService';
import { successResult, failureResult, errorResult } from '../services/RequestResult';
import { discover } from '../services/ProjectsService';
import { HatSearchParam } from '../comps/search/HatSearchParam';
import { _ } from 'lodash';
import BackToTop from '../comps/discover/BackToTop';
import BorderlessButton from '../comps/buttons/BorderlessButton'

const Search = () => {

    const [search, setSearch] = useSearchParams();
    const { getAccessTokenSilently } = useAuth0();

    const [ownHats, setOwnHats] = useState(undefined);

    const [keyword, setKeyword] = useState(search.get('keyword') ?? undefined);
    const [sort, setSort] = useState(search.get('sort') ?? undefined);
    const [hatType, setHatType] = useState(search.get('hatType') ?? undefined);

    const [searchRecommended, setSearchRecommended] = useState(false);
    const [keywordTyped, setKeywordTyped] = useState(undefined);

    const [pagedList, setPagedList] = useState(undefined);
    const [projectsShown, setProjectsShown] = useState(0);

    const [pageLoading, setPageLoading] = useState(true);
    const [projectsLoading, setProjectsLoading] = useState(true);
    const [loadMoreLoading, setLoadMoreLoading] = useState(false);

    const pageTopRef = useRef();

    useEffect(() => {
        const fetchOwnHats = () => {
            (async () => {
                setPageLoading(true);

                try {
                    let token = await getAccessTokenSilently({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await me(token);
                    setPageLoading(false);

                    if (result.outcome === successResult) {
                        setOwnHats(result.payload.hats);
                    } else {
                        console.log("error fetching users hats");
                    }
                } catch (ex) {
                    console.log(ex);
                } finally {
                    setPageLoading(false);
                }
            })();
        }

        fetchOwnHats();
    }, [])

    useEffect(() => {
        setProjectsShown(0);
        setKeyword(search.get('keyword') ?? undefined);
        setSort(search.get('sort') ?? undefined);
        setHatType(search.get('hatType') ?? undefined);
        setSearchRecommended(search.get('hatType') != null && search.get('hatType') != undefined)
    }, [search]);

    useEffect(() => {
        setKeywordTyped(keyword);
        
        let currentKeyword = search.get('keyword') ?? undefined;
        if (keyword != currentKeyword) {
            let newSearchParams = new URLSearchParams(search.toString());
            newSearchParams.delete('keyword');

            if (keyword)
                newSearchParams.set('keyword', keyword);

            setSearch(newSearchParams);
        }
    }, [keyword])    

    useEffect(() => {
      const setKeywordParamDebounced = setTimeout(() => {
        let newSearchParams = new URLSearchParams(search.toString);
        newSearchParams.delete('keyword');

        if (keywordTyped)
            newSearchParams.set('keyword', keywordTyped);

        setSearch(newSearchParams);
      }, 500);
    
      return () => {
        clearTimeout(setKeywordParamDebounced);
      }
    }, [keywordTyped])
    

    useEffect(() => {
        const handleDiscoveryRefinementsChange = () => {
            (async () => {
                try {
                    if (!ownHats)
                        return;

                    let hat = ownHats.find(h => h.type == hatType);

                    setProjectsLoading(true);

                    let token = await getAccessTokenSilently({
                        audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                    });

                    let result = await discover(keyword, sort, hat, token);
                    setProjectsLoading(false);

                    if (result.outcome === successResult) {
                        var projects = result.payload;
                        setPagedList(projects);
                        setProjectsShown(projects.items.length);
                    } else if (result.outcome === failureResult) {
                        console.log("failure");
                    } else if (result.outcome === errorResult) {
                        console.log("error");
                    }
                } catch (ex) {
                    console.log("exception", ex);
                } finally {
                    setProjectsLoading(false);
                }
            })();
        }

        handleDiscoveryRefinementsChange();
    }, [keyword, sort, hatType, ownHats])

    function loadNextPage(page) {
        (async () => {
            try {
                if (!ownHats)
                    return;

                let hat = ownHats.find(h => h.type == hatType);

                setLoadMoreLoading(true);

                let token = await getAccessTokenSilently({
                    audience: process.env.REACT_APP_EDU4_API_IDENTIFIER
                });

                let result = await discover(keyword, sort, hat, token, page);
                setLoadMoreLoading(false);

                if (result.outcome === successResult) {
                    var projects = result.payload;
                    setPagedList({
                        ...projects,
                        items: [...pagedList.items, ...projects.items]
                    });
                    setProjectsShown(projectsShown + projects.items.length);
                } else if (result.outcome === failureResult) {
                    console.log("failure");
                } else if (result.outcome === errorResult) {
                    console.log("error");
                }
            } catch (ex) {
                console.log("exception", ex);
            } finally {
                setProjectsLoading(false);
            }
        })();
    }

    if (pageLoading) {
        return (
            <SpinnerLayout>
                <BeatLoader
                    loading={pageLoading}
                    size={24}
                    color="blue">
                </BeatLoader>
            </SpinnerLayout>
        )
    }

    const searchResults = projectsLoading ?
        <BeatLoader></BeatLoader> :
        pagedList && pagedList.items && ownHats ?
            <div className='flex flex-col space-y-8'>
                {
                    pagedList.items.map((p, index) => <div key={index}>
                        <ProjectCard project={p} ownHats={ownHats}></ProjectCard>
                    </div>)
                }

                <div className='flex flex-col items-center gap-y-4'>
                    <p className='uppercase text-gray-500'>{`Showing ${pagedList.items.length} of ${pagedList.totalItems} projects`}</p>

                    {
                        pagedList.nextPage && !loadMoreLoading &&
                        <div className='h-24'>
                            <BorderlessButton text="Load more" onClick={() => loadNextPage(pagedList.nextPage)}></BorderlessButton>
                        </div>
                    }

                    {
                        pagedList.nextPage && loadMoreLoading &&
                        <div className='h-24'>
                            <BeatLoader></BeatLoader>
                        </div>
                    }
                </div>
            </div> :
            <p>There are currently no projects satisfying the criteria.</p>;

    return (
        ownHats &&

        <SingleColumnLayout
            title="Discover projects"
            description="Search projects and find opportunities to contribute to bla bla">
            <div className='flex flex-col gap-y-16 relative'>
                <div className='relative flex drop-shadow-md rounded-full bg-white px-6 py-4 gap-x-4 items-center mx-auto w-1/2'>
                    <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="lightgray" className="w-6 h-6 absolute left-6">
                        <path strokeLinecap="round" strokeLinejoin="round" d="m21 21-5.197-5.197m0 0A7.5 7.5 0 1 0 5.196 5.196a7.5 7.5 0 0 0 10.607 10.607Z" />
                    </svg>
                    <input
                        ref={pageTopRef}
                        className='text-center w-full'
                        value={keywordTyped}
                        onChange={e => setKeywordTyped(e.target.value)}>
                    </input>
                </div>

                <div className='flex flex-col gap-y-4'>
                    <div className='flex justify-between items-center'>
                        <label className="inline-flex items-center cursor-pointer gap-x-2">
                            <input type="checkbox" value={searchRecommended} className="sr-only peer" checked={searchRecommended} onChange={() => {
                                let newValue = !searchRecommended;
                                if (!newValue) {
                                    let newSearchParams = new URLSearchParams(search.toString());
                                    newSearchParams.delete('hatType');
                                    setSearch(newSearchParams);
                                }

                                setSearchRecommended(newValue);
                            }} />
                            <div className="relative w-11 h-6 bg-gray-200 rounded-full peer peer-checked:after:translate-x-full rtl:peer-checked:after:-translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:start-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all dark:border-gray-600 peer-checked:bg-indigo-500"></div>
                            <span className="text-gray-700">Recommended only</span>
                        </label>

                        <select
                            value={sort}
                            onChange={e => {
                                let newSearchParams = new URLSearchParams(search.toString());
                                newSearchParams.delete('sort');
                                if (e.target.value != "undefined")
                                    newSearchParams.set('sort', e.target.value);

                                setSearch(newSearchParams);
                            }}
                            className='rounded-full border-gray-200 text-gray-700 text-base'>
                            <option value="undefined">Default sort</option>
                            <option value="asc">Oldest posted first</option>
                            <option value="desc">Newest posted first</option>
                        </select>
                    </div>

                    <div className={`flex gap-x-4 items-center text-gray-600 ${searchRecommended ? "visible" : "hidden"}`}>
                        Looking for a
                        {
                            ownHats.map(h => (
                                <HatSearchParam
                                    selected={hatType != undefined && hatType == h.type}
                                    onSelected={() => {
                                        let newSearchParams = new URLSearchParams(search.toString());
                                        newSearchParams.delete('hatType');
                                        newSearchParams.set('hatType', h.type);
                                        setSearch(newSearchParams);
                                    }}>
                                    {h.type}
                                </HatSearchParam>
                            ))
                        }
                        like me
                    </div>

                    {searchResults}

                </div>

                <div className='fixed bottom-16 right-16 z-50'>
                    <BackToTop onClick={() => pageTopRef.current.scrollIntoView()}></BackToTop>
                </div>
            </div>
        </SingleColumnLayout >
    )
}

export default Search